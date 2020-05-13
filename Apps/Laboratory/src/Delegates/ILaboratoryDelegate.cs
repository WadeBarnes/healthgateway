﻿//-------------------------------------------------------------------------
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
namespace HealthGateway.Laboratory.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HealthGateway.Common.Models;
    using HealthGateway.Laboratory.Models;

    /// <summary>
    /// Interface that defines a delegate to retrieve laboratory information.
    /// </summary>
    public interface ILaboratoryDelegate
    {
        /// <summary>
        /// Returns a List of Covid Lab reports for the authenticated user.
        /// A Lab Report represents the overall Lab order.
        /// It has a collection of one or more Lab Results depending on the tests ordered.
        /// A Lab Report also has a Lab Report Document in PDF form.
        /// </summary>
        /// <param name="bearerToken">The security token representing the authenticated user.</param>
        /// <param name="pageIndex">The page index to return.</param>
        /// <returns>The list of Lab Reports available for the user identified by the bearerToken.</returns>
        Task<RequestResult<IEnumerable<LaboratoryReport>>> GetLaboratoryReports(string bearerToken, int pageIndex = 0);

        /// <summary>
        /// Gets the Lab report in binary format for the supplied id belonging to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the lab report to get.</param>
        /// <param name="bearerToken">The security token representing the authenticated user.</param>
        /// <returns>A base64 encoded PDF.</returns>
        Task<RequestResult<LaboratoryBinaryReport>> GetLabReport(Guid id, string bearerToken);
    }
}
