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
namespace HealthGateway.WebClient.Test.Controllers
{
    using DeepEqual.Syntax;
    using HealthGateway.Common.Models;
    using HealthGateway.Common.Models.CDogs;
    using HealthGateway.WebClient.Controllers;
    using HealthGateway.WebClient.Models;
    using HealthGateway.WebClient.Services;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// ReportController's Unit Tests.
    /// </summary>
    public class ReportControllerTests
    {
        /// <summary>
        /// Successfully Generate a Report - Happy Path scenario.
        /// </summary>
        [Fact]
        public void ShouldGetReport()
        {
            ReportRequestModel request = new ReportRequestModel()
            {
                Data = default(System.Text.Json.JsonElement),
                Template = TemplateType.Medication,
                Type = ReportFormatType.PDF,
            };

            RequestResult<ReportModel> expectedResult = new RequestResult<ReportModel>()
            {
                ResourcePayload = new ReportModel()
                {
                    Data = "123",
                },
                ResultStatus = Common.Constants.ResultType.Success,
            };

            Mock<IReportService> reportServiceMock = new Mock<IReportService>();
            reportServiceMock.Setup(s => s.GetReport(request)).Returns(expectedResult);

            ReportController controller = new ReportController(reportServiceMock.Object);
            var actualResult = controller.GenerateReport(request);

            Assert.True(((JsonResult)actualResult).Value.IsDeepEqual(expectedResult));
        }
    }
}
