using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using timeRecorder.Common.Model;
using timeRecorder.Function.Entities;

namespace timeRecorder.Test.Helpers
{
    public class TestFactory
    {
        public static TimeRecorderEntity getTimeEntity()
        {
            return new TimeRecorderEntity
            {
                ETag = "*",
                PartitionKey = "Timer",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployee = 1,
                Registry = DateTime.UtcNow,
                RegistryType = 0,
                WrapRegistries = false
            };
        }

        internal static DefaultHttpRequest CreateHttpRequest(Timer timerRequest)
        {
            string request = JsonConvert.SerializeObject(timerRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {

            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid timerId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{timerId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid timerId, Timer timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{timerId}"
            };
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }

        public static Timer GetTimeRequest()
        {
            return new Timer
            {
                IdEmployee = 1,
                Registry = DateTime.UtcNow,
                RegistryType = 0,
                WrapRegistries = false
            };

        }
        public static List<TimeRecorderEntity> GetListEntities()
        {
            return new List<TimeRecorderEntity>();
        }

        private static Stream GenerateStreamFromString(string request)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(request);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
