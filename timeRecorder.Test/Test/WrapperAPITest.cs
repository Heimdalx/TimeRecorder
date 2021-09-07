using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using timeRecorder.Function.Function;
using timeRecorder.Test.Helpers;
using Xunit;

namespace timeRecorder.Test.Test
{
    public class WrapperAPITest
    {
        [Fact]
        public async Task GetWrappeRegistriesByDate_Should_Return_200()
        {

            //arrange
            MockCloudTableWrapper mockTable = new MockCloudTableWrapper(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ILogger logger = TestFactory.CreateLogger();
            string date = "2021/09/04";
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(date);

            //act
            IActionResult response = await WrapperAPI.GetWrapperByDate(request, mockTable, date, logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
