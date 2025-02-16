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
namespace HealthGateway.Common.Models.PHSA
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Representation of the load state sent by PHSA.
    /// </summary>
    public class PHSALoadState
    {
        /// <summary>
        /// Gets or sets a value indicating whether the PHSA Load State is in the RefreshInProgress status.
        /// </summary>
        [JsonPropertyName("refreshInProgress")]
        public bool RefreshInProgress { get; set; }

        /// <summary>
        /// Gets or sets the SignalR url.
        /// </summary>
        [JsonPropertyName("signalRUrl")]
        public Uri? SignalRUrl { get; set; }

        /// <summary>
        /// Gets or sets the SignalR method.
        /// </summary>
        [JsonPropertyName("signalRMethod")]
        public string? SignalRMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of milliseconds to wait before requerying.
        /// </summary>
        [JsonPropertyName("backOffMilliseconds")]
        public int BackOffMilliseconds { get; set; }
    }
}
