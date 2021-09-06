using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using timeRecorder.Common.Model;
using timeRecorder.Function.Entities;
using timeRecorder.Function.Function;
using timeRecorder.Test.Helpers;
using Xunit;

namespace timeRecorder.Test.Test
{
    public class TimeRecorderAPITest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateRegisterShouldReturn200()
        {
            //Arrange 

            Timer timer = TestFactory.GetTimeRequest();
            MockCloudTableTimer mockCloudTableTime = new MockCloudTableTimer
                (new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timer);


            //Act

            IActionResult response = await TimeRecorderAPI.CreateRegistry(request, mockCloudTableTime, logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }

        [Fact]
        public async void UpdateRegisterShouldReturn200()
        {
            //Arrange 

            Timer timer = TestFactory.GetTimeRequest();
            MockCloudTableTimer mockCloudTableTime = new MockCloudTableTimer
                (new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Guid Id = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Id, timer);


            //Act

            IActionResult response = await TimeRecorderAPI.UpdateRegistry
                (request, mockCloudTableTime, Id.ToString(), logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }

        [Fact]
        public async void DeleteRegisterShouldReturn200()
        {
            //Arrange 

            Timer timer = TestFactory.GetTimeRequest();
            MockCloudTableTimer mockCloudTableTime = new MockCloudTableTimer
                (new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Guid Id = Guid.NewGuid();
            TimeRecorderEntity timeRecorderEntity = TestFactory.getTimeEntity();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Id, timer);


            //Act

            IActionResult response = await TimeRecorderAPI.DeleteRegister
                (request, timeRecorderEntity, mockCloudTableTime, Id.ToString(), logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public void GetIdRegisterShouldReturn200()
        {
            //Arrange 

            Timer timer = TestFactory.GetTimeRequest();
            Guid Id = Guid.NewGuid();
            TimeRecorderEntity timeRecorderEntity = TestFactory.getTimeEntity();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(Id, timer);


            //Act

            IActionResult response = TimeRecorderAPI.GetRegistry
                (request, timeRecorderEntity, Id.ToString(), logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllRegisterShouldReturn200()
        {
            //Arrange 

            MockCloudTableTimer mockCloudTableTime = new MockCloudTableTimer
           (new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            //Act

            IActionResult response = await TimeRecorderAPI.GetAllRegistries
                (request, mockCloudTableTime, logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
