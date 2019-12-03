﻿// -------------------------------------------------------------------------
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
namespace HealthGateway.Medication.Models
{
    /// <summary>
    /// The HNClient message request.
    /// </summary>
    public class HNMessageRequest
    {
        /// <summary>
        /// Gets or sets the patient PHN.
        /// </summary>
        public string Phn { get; set; }

        /// <summary>
        /// Gets or sets the pharmacy id.
        /// </summary>
        public string PharmacyId { get; set; }

        /// <summary>
        /// Gets or sets the requester Id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the requester ip address.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the transaction trace Id.
        /// </summary>
        public long TraceId { get; set; }

        /// <summary>
        /// Gets or sets the patient protective word.
        /// </summary>
        public string ProtectiveWord { get; set; }
    }
}