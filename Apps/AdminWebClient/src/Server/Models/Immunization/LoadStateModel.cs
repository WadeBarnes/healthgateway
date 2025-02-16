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
namespace HealthGateway.Admin.Models.Immunization
{
    using System.Text.Json.Serialization;
    using HealthGateway.Common.Models.PHSA;

    /// <summary>
    /// The Load State record data model.
    /// </summary>
    public class LoadStateModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Load State is in the RefreshInProgress status.
        /// </summary>
        [JsonPropertyName("refreshInProgress")]
        public bool RefreshInProgress { get; set; }

        /// <summary>
        /// Creates a Load State Model object from a PHSA model.
        /// </summary>
        /// <param name="model">The Load State to convert.</param>
        /// <returns>A LoadStateModel object.</returns>
        public static LoadStateModel FromPHSAModel(PHSALoadState model)
        {
            LoadStateModel returnValue = new ();
            returnValue.RefreshInProgress = model.RefreshInProgress;
            return returnValue;
        }
    }
}
