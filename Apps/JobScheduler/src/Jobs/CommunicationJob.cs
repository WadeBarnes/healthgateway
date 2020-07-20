// -------------------------------------------------------------------------
//  Copyright © 2019 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------
namespace Healthgateway.JobScheduler.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Hangfire;
    using HealthGateway.Common.Jobs;
    using HealthGateway.Common.Services;
    using HealthGateway.Database.Constants;
    using HealthGateway.Database.Context;
    using HealthGateway.Database.Delegates;
    using HealthGateway.Database.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    public class CommunicationJob : ICommunicationJob
    {
        private const int ConcurrencyTimeout = 30 * 60; // 30 minutes
        private readonly IConfiguration configuration;
        private readonly ILogger<CommunicationJob> logger;
        private readonly ICommunicationDelegate communicationDelegate;
        private readonly ICommunicationEmailDelegate commEmailDelegate;
        private readonly IEmailQueueService emailQueueService;
        private readonly GatewayDbContext dbContext;

        private readonly int retryFetchSize;
        private readonly string fromEmailAddressHGDonotreply;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationJob"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="communicationDelegate">The Communication delegate to use.</param>
        /// <param name="commEmailDelegate">The Communication Email delegate to use.</param>
        /// <param name="emailQueueService">The email queue service to use.</param>
        /// <param name="dbContext">The db context to use.</param>
        public CommunicationJob(IConfiguration configuration, ILogger<CommunicationJob> logger, ICommunicationDelegate communicationDelegate, ICommunicationEmailDelegate commEmailDelegate, IEmailQueueService emailQueueService, GatewayDbContext dbContext)
        {
            Contract.Requires((configuration != null) && (emailQueueService != null));
            this.configuration = configuration!;
            this.logger = logger;
            this.communicationDelegate = communicationDelegate!;
            this.commEmailDelegate = commEmailDelegate!;
            this.emailQueueService = emailQueueService!;
            this.dbContext = dbContext;

            IConfigurationSection emailJobSection = this.configuration.GetSection("EmailJob");
            this.retryFetchSize = emailJobSection.GetValue<int>("MaxRetryFetchSize", 250);
            IConfigurationSection commEmailJobSection = this.configuration.GetSection("CreateCommEmailsForNewCommunications");
            this.fromEmailAddressHGDonotreply = commEmailJobSection.GetValue<string>("FromEmailAddressHGDonotreply");
        }

        /// <inheritdoc />
        [DisableConcurrentExecution(ConcurrencyTimeout)]
        public void CreateCommunicationEmailsForNewCommunications()
        {
            this.logger.LogDebug($"Creating emails & communication emails...");
            List<Communication> communications = this.communicationDelegate.GetCommunicationsByTypeAndStatusCode(CommunicationType.Email, CommunicationStatus.New);

            if (communications.Count > 0)
            {
                this.logger.LogInformation($"Found {communications.Count} communications which are in New status.");

                foreach (Communication comm in communications)
                {
#pragma warning disable CA1031 //We want to catch exception.
                    try
                    {
                        DateTime? createdOnOrAfterFilter = null;
                        string lastProcessedProfileHdid = string.Empty;
                        List<UserProfile>? usersToSendCommEmails = null;
                        do
                        {
                            if (usersToSendCommEmails != null && usersToSendCommEmails.Count > 0)
                            {
                                lastProcessedProfileHdid = usersToSendCommEmails[usersToSendCommEmails.Count - 1].HdId;
                            }

                            usersToSendCommEmails = this.commEmailDelegate.GetActiveUserProfilesWithoutCommEmailByCommunicationIdAndByCreatedOnOrAfter(comm.Id, createdOnOrAfterFilter, this.retryFetchSize);
                            foreach (UserProfile profile in usersToSendCommEmails)
                            {
                                // Idea for Improvement: we can look into a lib for Bulk Insert / Update using .NET Core for Postgres (if needed).
                                // Insert a new Email record into db.
                                Email email = new Email()
                                {
                                    From = this.fromEmailAddressHGDonotreply,
                                    To = profile.Email,
                                    Subject = comm.Subject,
                                    Body = comm.Text,
                                    FormatCode = EmailFormat.HTML,
                                    Priority = comm.Priority,
                                };
                                this.emailQueueService.QueueNewEmail(email, false);

                                // Inser a new CommunicationEmail record into
                                CommunicationEmail commEmail = new CommunicationEmail()
                                {
                                    Email = email,
                                    UserProfile = profile,
                                    Communication = comm,
                                };
                                this.commEmailDelegate.Add(commEmail, false);
                            }

                            if (usersToSendCommEmails.Count > 0)
                            {
                                // Set the createdOnOrAfterFilter for the next query retrieving the next chunk of user profiles.
                                createdOnOrAfterFilter = usersToSendCommEmails[usersToSendCommEmails.Count - 1].CreatedDateTime;
                            }

                            this.dbContext.SaveChanges(); // commit after every retryFetchSize (or 250) pairs of Email & CommunicationEmail.
                        }
                        while (usersToSendCommEmails.Count == this.retryFetchSize
                               && lastProcessedProfileHdid != usersToSendCommEmails[usersToSendCommEmails.Count - 1].HdId); // keep looping when the above query returns the max return rows (or user profiles).

                        // Update Communication Status to Processed
                        comm.CommunicationStatusCode = CommunicationStatus.Processed;
                        this.communicationDelegate.Update(comm, true);
                    }
                    catch (Exception e)
                    {
                        // log the exception as a warning but we can continue
                        this.logger.LogWarning($"Error while creating new emails and communication email records for Email Communication - skipping for now\n{e.ToString()}");
                    }
#pragma warning restore CA1031 // Restore warnings.
                }
            }
        }
    }
}
