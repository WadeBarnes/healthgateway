//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
// <auto-generated />
namespace HealthGateway.WebClient.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DeepEqual.Syntax;
    using HealthGateway.Common.Constants;
    using HealthGateway.Common.Delegates;
    using HealthGateway.Common.ErrorHandling;
    using HealthGateway.Common.Models;
    using HealthGateway.Common.Services;
    using HealthGateway.Database.Constants;
    using HealthGateway.Database.Delegates;
    using HealthGateway.Database.Models;
    using HealthGateway.Database.Wrapper;
    using HealthGateway.WebClient.Constant;
    using HealthGateway.WebClient.Models;
    using HealthGateway.WebClient.Services;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class UserProfileServiceTest
    {
        readonly string hdid = "1234567890123456789012345678901234567890123456789012";

        private Tuple<RequestResult<UserProfileModel>, UserProfileModel> ExecuteGetUserProfile(Database.Constants.DBStatusCode dbResultStatus = Database.Constants.DBStatusCode.Read, DateTime? lastLoginDateTime = null)
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
                LastLoginDateTime = lastLoginDateTime,
            };

            DBResult<UserProfile> userProfileDBResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = dbResultStatus
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);
            if(lastLoginDateTime != null)
                expected.HasTermsOfServiceUpdated = true;

            LegalAgreement termsOfService = new LegalAgreement()
            {
                Id = Guid.NewGuid(),
                LegalText = "",
                EffectiveDate = DateTime.Now
            };

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.GetUserProfile(hdid)).Returns(userProfileDBResult);
            profileDelegateMock.Setup(s => s.Update(userProfile, true)).Returns(userProfileDBResult);

            UserPreference dbUserPreference = new UserPreference
            {
                HdId = hdid,
                Preference = "TutorialPopover",
                Value = true.ToString(),
            };
            List<UserPreference> userPreferences = new List<UserPreference>();
            userPreferences.Add(dbUserPreference);
            DBResult<IEnumerable<UserPreference>> readResult = new DBResult<IEnumerable<UserPreference>>
            {
                Payload = userPreferences,
                Status = DBStatusCode.Read
            };
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();
            preferenceDelegateMock.Setup(s => s.GetUserPreferences(hdid)).Returns(readResult);

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration());

            Mock<ILegalAgreementDelegate> legalAgreementDelegateMock = new Mock<ILegalAgreementDelegate>();
            legalAgreementDelegateMock
                .Setup(s => s.GetActiveByAgreementType(LegalAgreementType.TermsofService))
                .Returns(new DBResult<LegalAgreement>() { Payload = termsOfService });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                legalAgreementDelegateMock.Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object,
                new Mock<IPatientService>().Object);

            RequestResult<UserProfileModel> actualResult = service.GetUserProfile(hdid, DateTime.Now);

            return new Tuple<RequestResult<UserProfileModel>, UserProfileModel>(actualResult, expected);
        }

        [Fact]
        public void ShouldGetUserProfile()
        {
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteGetUserProfile(Database.Constants.DBStatusCode.Read);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.Equal(hdid, expectedRecord.HdId);
        }

        [Fact]
        public void ShouldGetUserProfileWithTermsOfServiceUpdated()
        {
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteGetUserProfile(Database.Constants.DBStatusCode.Read, DateTime.Today);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.True(actualResult.ResourcePayload.HasTermsOfServiceUpdated);
        }

        [Fact]
        public void ShouldGetUserProfileWithDBError()
        {
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteGetUserProfile(Database.Constants.DBStatusCode.Error);
            var actualResult = result.Item1;

            Assert.Equal(Common.Constants.ResultType.Error, actualResult.ResultStatus);
            Assert.Equal("testhostServer-CI-DB", actualResult.ResultError.ErrorCode);
        }

        [Fact]
        public void ShouldGetUserProfileWithProfileNotFoundError()
        {
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteGetUserProfile(Database.Constants.DBStatusCode.NotFound);
            var actualResult = result.Item1;

            Assert.Equal(Common.Constants.ResultType.Success, actualResult.ResultStatus);
            Assert.Null(actualResult.ResourcePayload.HdId);
        }

        private async Task<Tuple<RequestResult<UserProfileModel>, UserProfileModel>> ExecuteInsertUserProfile(Database.Constants.DBStatusCode dbResultStatus, string registrationStatus, string inviteCode, MessagingVerification messagingVerification = null)
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
                Email = "unit.test@hgw.ca"
            };

            DBResult<UserProfile> insertResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = DBStatusCode.Created
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.InsertUserProfile(userProfile)).Returns(insertResult);
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            if (messagingVerification == null)
                messagingVerification = new MessagingVerification();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(messagingVerification);

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration() { WebClient = new WebClientConfiguration() { RegistrationStatus = registrationStatus } });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            cryptoDelegateMock.Setup(s => s.GenerateKey()).Returns("abc");

            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            notificationServiceMock.Setup(s => s.QueueNotificationSettings(It.IsAny<NotificationSettingsRequest>()));

            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();
            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                new Mock<ILegalAgreementDelegate>().Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object,
                new Mock<IPatientService>().Object);

            RequestResult<UserProfileModel> actualResult = await service.CreateUserProfile(new CreateUserRequest() { Profile = userProfile, InviteCode = inviteCode }, new Uri("http://localhost/"), "bearer_token");

            return new Tuple<RequestResult<UserProfileModel>, UserProfileModel>(actualResult, expected);
        }

        [Fact]
        public async void ShouldInsertUserProfile()
        {
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = await ExecuteInsertUserProfile(DBStatusCode.Created, RegistrationStatus.Open, null);
            var actualResult = result.Item1;
            var expected = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
        }

        [Fact]
        public async void ShouldInsertUserProfileWithClosedRegistration()
        {
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = await ExecuteInsertUserProfile(DBStatusCode.Error, RegistrationStatus.Closed, null);
            var actualResult = result.Item1;

            Assert.Equal(ResultType.Error, actualResult.ResultStatus);
            Assert.Equal(ErrorTranslator.InternalError(ErrorType.InvalidState), actualResult.ResultError.ErrorCode);
            Assert.Equal("Registration is closed", actualResult.ResultError.ResultMessage);
        }

        [Fact]
        public async void ShouldInsertUserProfileWithValidInviteEmail()
        {
            var inviteCode = Guid.NewGuid().ToString();
            var messagingVerification = new MessagingVerification()
            {
                Email = new Email() {
                    To = "unit.test@hgw.ca",
                },
                Validated = false,
            };
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = await ExecuteInsertUserProfile(DBStatusCode.Created, RegistrationStatus.InviteOnly, inviteCode, messagingVerification);
            var actualResult = result.Item1;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
        }

        [Fact]
        public async void ShouldInsertUserProfileWithInvalidInviteEmail()
        {
            var inviteCode = Guid.NewGuid().ToString();
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = await ExecuteInsertUserProfile(DBStatusCode.Created, RegistrationStatus.InviteOnly, inviteCode);
            var actualResult = result.Item1;

            Assert.Equal(ResultType.Error, actualResult.ResultStatus);
            Assert.Equal(ErrorTranslator.InternalError(ErrorType.InvalidState), actualResult.ResultError.ErrorCode);
            Assert.Equal("Invalid email invite", actualResult.ResultError.ResultMessage);
        }

        [Fact]
        public async void ShouldInsertUserProfileWithInvalidInviteCode()
        {
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = await ExecuteInsertUserProfile(DBStatusCode.Created, RegistrationStatus.InviteOnly, null);
            var actualResult = result.Item1;

            Assert.Equal(ResultType.Error, actualResult.ResultStatus);
            Assert.Equal(ErrorTranslator.InternalError(ErrorType.InvalidState), actualResult.ResultError.ErrorCode);
            Assert.Equal("Invalid email invite", actualResult.ResultError.ResultMessage);
        }

        [Fact]
        public async void ShouldQueueNotificationUpdate()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = "1234567890123456789012345678901234567890123456789012",
                AcceptedTermsOfService = true,
                Email = string.Empty
            };

            DBResult<UserProfile> insertResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = DBStatusCode.Created
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.InsertUserProfile(userProfile)).Returns(insertResult);
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration() { WebClient = new WebClientConfiguration() { RegistrationStatus = RegistrationStatus.Open } });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            cryptoDelegateMock.Setup(s => s.GenerateKey()).Returns("abc");

            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            notificationServiceMock.Setup(s => s.QueueNotificationSettings(It.IsAny<NotificationSettingsRequest>()));

            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                new Mock<ILegalAgreementDelegate>().Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object,
                new Mock<IPatientService>().Object);

            RequestResult<UserProfileModel> actualResult = await service.CreateUserProfile(new CreateUserRequest() { Profile = userProfile }, new Uri("http://localhost/"), "bearer_token");
            notificationServiceMock.Verify(s => s.QueueNotificationSettings(It.IsAny<NotificationSettingsRequest>()), Times.Once());
            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.True(actualResult.ResourcePayload.IsDeepEqual(expected));
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldValidateAgeAsync()
        {
            string hdid = "1234567890123456789012345678901234567890123456789012";
            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration() { WebClient = new WebClientConfiguration() { MinPatientAge = 19 } });
            PatientModel patientModel = new PatientModel()
            {
                Birthdate = DateTime.Now.AddYears(-15)
            };
            Mock<IPatientService> patientServiceMock = new Mock<IPatientService>();
            patientServiceMock.Setup(s => s.GetPatient(hdid, PatientIdentifierType.HDID)).ReturnsAsync(new RequestResult<PatientModel> { ResultStatus = ResultType.Success, ResourcePayload = patientModel });

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                new Mock<IUserProfileDelegate>().Object,
                new Mock<IUserPreferenceDelegate>().Object,
                new Mock<IEmailDelegate>().Object,
                new Mock<IMessagingVerificationDelegate>().Object,
                configServiceMock.Object,
                new Mock<IEmailQueueService>().Object,
                new Mock<ILegalAgreementDelegate>().Object,
                new Mock<ICryptoDelegate>().Object,
                new Mock<INotificationSettingsService>().Object,
                new Mock<IMessagingVerificationDelegate>().Object,
                patientServiceMock.Object
            );

            PrimitiveRequestResult<bool> expected = new PrimitiveRequestResult<bool>() { ResultStatus = ResultType.Success, ResourcePayload = false };
            PrimitiveRequestResult<bool> actualResult = await service.ValidateMinimumAge(hdid);
            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.Equal(expected.ResourcePayload, actualResult.ResourcePayload);
        }

        private Tuple<RequestResult<Dictionary<string, UserPreferenceModel>>, List<UserPreferenceModel>> ExecuteGetUserPreference(Database.Constants.DBStatusCode dbResultStatus = Database.Constants.DBStatusCode.Read)
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
            };

            DBResult<UserProfile> userProfileDBResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = dbResultStatus
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            LegalAgreement termsOfService = new LegalAgreement()
            {
                Id = Guid.NewGuid(),
                LegalText = "",
                EffectiveDate = DateTime.Now
            };

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.GetUserProfile(hdid)).Returns(userProfileDBResult);
            profileDelegateMock.Setup(s => s.Update(userProfile, true)).Returns(userProfileDBResult);

            UserPreferenceModel userPreferenceModel = new UserPreferenceModel
            {
                HdId = hdid,
                Preference = "TutorialPopover",
                Value = true.ToString(),
            };

            List<UserPreferenceModel> userPreferences = new List<UserPreferenceModel>();
            userPreferences.Add(userPreferenceModel);

            List<UserPreference> dbUserPreferences = new List<UserPreference>();
            dbUserPreferences.Add(userPreferenceModel.ToDbModel());

            DBResult<IEnumerable<UserPreference>> readResult = new DBResult<IEnumerable<UserPreference>>
            {
                Payload = dbUserPreferences,
                Status = dbResultStatus
            };
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();
            preferenceDelegateMock.Setup(s => s.GetUserPreferences(hdid)).Returns(readResult);

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration());

            Mock<ILegalAgreementDelegate> legalAgreementDelegateMock = new Mock<ILegalAgreementDelegate>();
            legalAgreementDelegateMock
                .Setup(s => s.GetActiveByAgreementType(LegalAgreementType.TermsofService))
                .Returns(new DBResult<LegalAgreement>() { Payload = termsOfService });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                legalAgreementDelegateMock.Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object,
                new Mock<IPatientService>().Object);

            RequestResult<Dictionary<string, UserPreferenceModel>> actualResult = service.GetUserPreferences(hdid);

            return new Tuple<RequestResult<Dictionary<string, UserPreferenceModel>>, List<UserPreferenceModel>>(actualResult, userPreferences);
        }

        [Fact]
        public void ShouldGetUserPreference()
        {
            Tuple<RequestResult<Dictionary<string, UserPreferenceModel>>, List<UserPreferenceModel>> result = ExecuteGetUserPreference(Database.Constants.DBStatusCode.Read);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.Equal(actualResult.ResourcePayload.Count, expectedRecord.Count);
            Assert.Equal(actualResult.ResourcePayload["TutorialPopover"].Value, expectedRecord[0].Value);
        }

        [Fact]
        public void ShouldGetUserPreferenceWithDBError()
        {
            Tuple<RequestResult<Dictionary<string, UserPreferenceModel>>, List<UserPreferenceModel>> result = ExecuteGetUserPreference(Database.Constants.DBStatusCode.Error);
            var actualResult = result.Item1;

            Assert.Equal(Common.Constants.ResultType.Error, actualResult.ResultStatus);
            Assert.Equal(ErrorTranslator.ServiceError(ErrorType.CommunicationInternal, ServiceType.Database), actualResult.ResultError.ErrorCode);
        }

        private RequestResult<UserPreferenceModel> ExecuteUpdateUserPreference(Database.Constants.DBStatusCode dbResultStatus = Database.Constants.DBStatusCode.Updated)
        {
            UserPreferenceModel userPreferenceModel = new UserPreferenceModel
            {
                HdId = hdid,
                Preference = "TutorialPopover",
                Value = "mocked value",
            };
            DBResult<UserPreference> readResult = new DBResult<UserPreference>
            {
                Payload = userPreferenceModel.ToDbModel(),
                Status = dbResultStatus
            };
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();

            preferenceDelegateMock.Setup(s => s.UpdateUserPreference(It.IsAny<UserPreference>(), It.IsAny<bool>())).Returns(readResult);

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();

            Mock<ILegalAgreementDelegate> legalAgreementDelegateMock = new Mock<ILegalAgreementDelegate>();
            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                legalAgreementDelegateMock.Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object,
                new Mock<IPatientService>().Object);

            return service.UpdateUserPreference(userPreferenceModel);
        }

        [Fact]
        public void ShouldCreateUserPreference()
        {
            RequestResult<UserPreferenceModel> result = ExecuteUpdateUserPreference(Database.Constants.DBStatusCode.Created);

            Assert.Equal(ResultType.Success, result.ResultStatus);
        }

        [Fact]
        public void ShouldUpdateUserPreference()
        {
            RequestResult<UserPreferenceModel> result = ExecuteUpdateUserPreference(Database.Constants.DBStatusCode.Updated);

            Assert.Equal(ResultType.Success, result.ResultStatus);
        }

        private Tuple<RequestResult<UserProfileModel>, UserProfileModel> ExecuteCloseUserProfile(UserProfile userProfile, Database.Constants.DBStatusCode dbResultStatus = Database.Constants.DBStatusCode.Read)
        {
            DBResult<UserProfile> userProfileDBResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = dbResultStatus
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            LegalAgreement termsOfService = new LegalAgreement()
            {
                Id = Guid.NewGuid(),
                LegalText = "",
                EffectiveDate = DateTime.Now
            };

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            emailer.Setup(s => s.QueueNewEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), false));

            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.GetUserProfile(hdid)).Returns(userProfileDBResult);
            profileDelegateMock.Setup(s => s.Update(userProfile, true)).Returns(userProfileDBResult);

            UserPreference dbUserPreference = new UserPreference
            {
                HdId = hdid,
                Preference = "TutorialPopover",
                Value = true.ToString(),
            };
            List<UserPreference> userPreferences = new List<UserPreference>();
            userPreferences.Add(dbUserPreference);
            DBResult<IEnumerable<UserPreference>> readResult = new DBResult<IEnumerable<UserPreference>>
            {
                Payload = userPreferences,
                Status = DBStatusCode.Read
            };
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();
            preferenceDelegateMock.Setup(s => s.GetUserPreferences(hdid)).Returns(readResult);

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration());

            Mock<ILegalAgreementDelegate> legalAgreementDelegateMock = new Mock<ILegalAgreementDelegate>();
            legalAgreementDelegateMock
                .Setup(s => s.GetActiveByAgreementType(LegalAgreementType.TermsofService))
                .Returns(new DBResult<LegalAgreement>() { Payload = termsOfService });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                legalAgreementDelegateMock.Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object,
                new Mock<IPatientService>().Object);

            RequestResult<UserProfileModel> actualResult = service.CloseUserProfile(hdid, Guid.NewGuid(), "127.0.0.1");

            return new Tuple<RequestResult<UserProfileModel>, UserProfileModel>(actualResult, expected);
        }

        [Fact]
        public void ShouldCloseUserProfile()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
            };
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteCloseUserProfile(userProfile, Database.Constants.DBStatusCode.Read);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.NotNull(actualResult.ResourcePayload.ClosedDateTime);
        }

        [Fact]
        public void PreviouslyClosedUserProfile()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
                ClosedDateTime = DateTime.Today
            };
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteCloseUserProfile(userProfile, Database.Constants.DBStatusCode.Read);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.Equal(userProfile.ClosedDateTime, actualResult.ResourcePayload.ClosedDateTime);
        }

        [Fact]
        public void ShouldCloseUserProfileAndQueueNewEmail()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
                Email = "unit.test@hgw.ca"
            };
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteCloseUserProfile(userProfile, Database.Constants.DBStatusCode.Read);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.NotNull(actualResult.ResourcePayload.ClosedDateTime);
        }


        private Tuple<RequestResult<UserProfileModel>, UserProfileModel> ExecuteRecoverUserProfile(UserProfile userProfile, Database.Constants.DBStatusCode dbResultStatus = Database.Constants.DBStatusCode.Read)
        {
            DBResult<UserProfile> userProfileDBResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = dbResultStatus
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            LegalAgreement termsOfService = new LegalAgreement()
            {
                Id = Guid.NewGuid(),
                LegalText = "",
                EffectiveDate = DateTime.Now
            };

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            emailer.Setup(s => s.QueueNewEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), false));

            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.GetUserProfile(hdid)).Returns(userProfileDBResult);
            profileDelegateMock.Setup(s => s.Update(userProfile, true)).Returns(userProfileDBResult);

            UserPreference dbUserPreference = new UserPreference
            {
                HdId = hdid,
                Preference = "TutorialPopover",
                Value = true.ToString(),
            };
            List<UserPreference> userPreferences = new List<UserPreference>();
            userPreferences.Add(dbUserPreference);
            DBResult<IEnumerable<UserPreference>> readResult = new DBResult<IEnumerable<UserPreference>>
            {
                Payload = userPreferences,
                Status = DBStatusCode.Read
            };
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();
            preferenceDelegateMock.Setup(s => s.GetUserPreferences(hdid)).Returns(readResult);

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration());

            Mock<ILegalAgreementDelegate> legalAgreementDelegateMock = new Mock<ILegalAgreementDelegate>();
            legalAgreementDelegateMock
                .Setup(s => s.GetActiveByAgreementType(LegalAgreementType.TermsofService))
                .Returns(new DBResult<LegalAgreement>() { Payload = termsOfService });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                legalAgreementDelegateMock.Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object,
                new Mock<IPatientService>().Object);

            RequestResult<UserProfileModel> actualResult = service.RecoverUserProfile(hdid, "127.0.0.1");

            return new Tuple<RequestResult<UserProfileModel>, UserProfileModel>(actualResult, expected);
        }

        [Fact]
        public void ShouldRecoverUserProfile()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
                ClosedDateTime = DateTime.Today,
                Email = "unit.test@hgw.ca"
            };
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteRecoverUserProfile(userProfile, Database.Constants.DBStatusCode.Read);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.Null(actualResult.ResourcePayload.ClosedDateTime);
        }

        [Fact]
        public void ShouldRecoverUserProfileAlreadyActive()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
                ClosedDateTime = null
            };
            Tuple<RequestResult<UserProfileModel>, UserProfileModel> result = ExecuteRecoverUserProfile(userProfile, Database.Constants.DBStatusCode.Read);
            var actualResult = result.Item1;
            var expectedRecord = result.Item2;

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.Null(actualResult.ResourcePayload.ClosedDateTime);
        }
    }
}
