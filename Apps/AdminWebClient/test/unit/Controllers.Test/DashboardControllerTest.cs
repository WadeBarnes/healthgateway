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
namespace HealthGateway.Admin.Test.Services
{
    using DeepEqual.Syntax;
    using HealthGateway.Admin.Controllers;
    using HealthGateway.Admin.Services;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for Admin Dashboard.
    /// </summary>
    public class DashboardControllerTest
    {
        /// <summary>
        /// GetUsersWithNotesCount should return the note count.
        /// </summary>
        [Fact]
        public void ShouldGetUsersWithNotesCount()
        {
            int expected = 10;
            Mock<IDashboardService> mockService = new Mock<IDashboardService>();
            mockService.Setup(s => s.GetUsersWithNotesCount()).Returns(expected);

            // Initialize controller
            DashboardController controller = new DashboardController(
                mockService.Object);

            // Test if controller adds communication properly
            IActionResult actualResult = controller.GetUsersWithNotesCount();
            Assert.IsType<JsonResult>(actualResult);
            Assert.True(((JsonResult)actualResult).Value.IsDeepEqual(expected));
        }

        /// <summary>
        /// GetUsersWithNotesCount should return the note count.
        /// </summary>
        [Fact]
        public void ShouldGetDependentCount()
        {
            int expected = 10;
            Mock<IDashboardService> mockService = new Mock<IDashboardService>();
            mockService.Setup(s => s.GetDependentCount()).Returns(expected);
            DashboardController controller = new DashboardController(
                mockService.Object);

            IActionResult actualResult = controller.GetDependentCount();
            Assert.IsType<JsonResult>(actualResult);
            Assert.True(((JsonResult)actualResult).Value.IsDeepEqual(expected));
        }
    }
}