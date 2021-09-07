using System;
using System.Collections.Generic;
using System.Text;
using timeRecorder.Function.Function;
using timeRecorder.Test.Helpers;
using Xunit;

namespace timeRecorder.Test.Test
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduleFunction_Should_Log_Message()
        {

            //arrange
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            MockCloudTableTimer timerTable = new MockCloudTableTimer(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableTimer wraperTable = new MockCloudTableTimer(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));


            //act
            _ = ScheduleFunction.Execute(null, timerTable, wraperTable, logger);
            string message = logger.Logs[0];

            //assert
            Assert.Contains("Wrapping data in table wrape", message);
        }
    }
}

