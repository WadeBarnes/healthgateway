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
namespace HealthGateway.Admin.Models.Immunization
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using HealthGateway.Common.Models.Immunization;

    /// <summary>
    /// Represents Immunization Result.
    /// </summary>
    public class ImmunizationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmunizationResult"/> class.
        /// </summary>
        public ImmunizationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmunizationResult"/> class.
        /// </summary>
        /// <param name="loadState">The load state model.</param>
        /// <param name="immunizations">The list of immunizations.</param>
        [JsonConstructor]
        public ImmunizationResult(LoadStateModel loadState, IList<ImmunizationEvent> immunizations)
        {
            this.LoadState = loadState;
            this.Immunizations = immunizations;
        }

        /// <summary>
        /// Gets or sets the Load State.
        /// </summary>
        [JsonPropertyName("loadState")]
        public LoadStateModel LoadState { get; set; } = new LoadStateModel();

        /// <summary>
        /// Gets the list of Immunizations events.
        /// </summary>
        [JsonPropertyName("immunizations")]
        public IList<ImmunizationEvent> Immunizations { get; } = new List<ImmunizationEvent>();
    }
}
