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
namespace HealthGateway.WebClient.Test.Services
{
    using System.Text.Json;
    using DeepEqual.Syntax;
    using HealthGateway.Common.Delegates;
    using HealthGateway.Common.Models;
    using HealthGateway.Common.Models.CDogs;
    using HealthGateway.WebClient.Models;
    using HealthGateway.WebClient.Services;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// ReportService's Unit Tests.
    /// </summary>
    public class ReportServiceTests
    {
        /// <summary>
        /// GetReport - Happy path scenario.
        /// </summary>
        [Fact]
        public void ShouldGetReport()
        {
            RequestResult<ReportModel> expectedResult = new RequestResult<ReportModel>
            {
                ResourcePayload = new ReportModel()
                {
                    Data = "base64data",
                },
                ResultStatus = Common.Constants.ResultType.Success,
            };

            ReportRequestModel reportRequest = new ReportRequestModel()
            {
                Data = JsonDocument.Parse("{}").RootElement,
                Template = TemplateType.Medication,
                Type = ReportFormatType.PDF,
            };

            Mock<ICDogsDelegate> cdogsDelegateMock = new Mock<ICDogsDelegate>();
            cdogsDelegateMock.Setup(s => s.GenerateReportAsync(It.Is<CDogsRequestModel>(r => r.Options.ReportName == "HealthGatewayMedicationReport"))).ReturnsAsync(expectedResult);

            IReportService service = new ReportService(
                new Mock<ILogger<ReportService>>().Object,
                cdogsDelegateMock.Object);
            RequestResult<ReportModel> actualResult = service.GetReport(reportRequest);

            Assert.Equal(Common.Constants.ResultType.Success, actualResult.ResultStatus);
            Assert.True(actualResult.IsDeepEqual(expectedResult));
        }
    }
}
