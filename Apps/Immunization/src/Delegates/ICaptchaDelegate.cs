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
namespace HealthGateway.Immunization.Delegates
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface that defines a delegate to validate a captcha token.
    /// </summary>
    public interface ICaptchaDelegate
    {
        /// <summary>
        /// Validates a captcha token.
        /// </summary>
        /// <param name="token">The captcha token.</param>
        /// <returns>A value indicating whether the token is valid or not.</returns>
        Task<bool> IsCaptchaValid(string token);
    }
}
