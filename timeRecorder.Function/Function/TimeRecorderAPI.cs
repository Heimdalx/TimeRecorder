using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using timeRecorder.Common.Model;
using timeRecorder.Common.Responses;
using timeRecorder.Function.Entities;

namespace timeRecorder.Function.Function
{
    public static class TimeRecorderAPI
    {
        [FunctionName(nameof(CreateRegistry))]
        public static async Task<IActionResult> CreateRegistry([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Timer")]
            HttpRequest req,[Table("Timer", Connection = "AzureWebJobsStorage")] CloudTable registriesTable,ILogger log)
        {
            log.LogInformation("Registry creation");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Timer time = JsonConvert.DeserializeObject<Timer>(requestBody);

            if (string.IsNullOrEmpty(time?.IdEmployee.ToString()) || time?.IdEmployee <= 0)

            {

                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Error, Id it can´t be null"
                });

            }

            if (string.IsNullOrEmpty(time?.Registry.ToString()))
            {

                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Error, Date it can´t be null"
                });
            }

            if (string.IsNullOrEmpty(time?.RegistryType.ToString()))
            {

                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Error, Type it can´t be null"
                });
            }

            TimeRecorderEntity timeRecorderEntity = new TimeRecorderEntity
            {
                IdEmployee = time.IdEmployee,
                Registry = time.Registry,
                RegistryType = time.RegistryType,
                WrapRegistries = false,
                ETag = "*",
                PartitionKey = "time",
                RowKey = Guid.NewGuid().ToString()
            };


            TableOperation createOperation = TableOperation.Insert(timeRecorderEntity);
            await registriesTable.ExecuteAsync(createOperation);

            string Message = "New registry done :D";
            log.LogInformation(Message);



            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = Message,
                Result = timeRecorderEntity
            });
        }

        [FunctionName(nameof(UpdateRegistry))]
        public static async Task<IActionResult> UpdateRegistry([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Timer/{Id}")] 
            HttpRequest req,[Table("Timer", Connection = "AzureWebJobsStorage")] CloudTable registriesTable,string Id,ILogger log)
        {
            log.LogInformation("Registry update");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Timer timer = JsonConvert.DeserializeObject<Timer>(requestBody);

            TableOperation findById = TableOperation.Retrieve<TimeRecorderEntity>("timer", Id);

            TableResult Result = await registriesTable.ExecuteAsync(findById);

            if (Result.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "ID not found :("

                });
            }

            TimeRecorderEntity timeRecord = (TimeRecorderEntity)Result.Result;

            if (!string.IsNullOrEmpty(timer.Registry.ToString()))
            {
                timeRecord.Registry = timer.Registry;
            }

            TableOperation update = TableOperation.Replace(timeRecord);
            await registriesTable.ExecuteAsync(update);

            string Message = $"Registry with ID {Id}, was successfully updated :D";

            log.LogInformation(Message);



            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = Message,
                Result = timeRecord
            });
        }

        [FunctionName(nameof(DeleteRegister))]
        public static async Task<IActionResult> DeleteRegister([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Timer/{Id}")] 
            HttpRequest req,[Table("Timer", "Timer", "{Id}", Connection = "AzureWebJobsStorage")] TimeRecorderEntity timeRecorderEntity,
            [Table("Timer", Connection = "AzureWebJobsStorage")] CloudTable timeTable,string Id,ILogger log)
        {
            log.LogInformation("Delete a registry");

            if (timeRecorderEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Registry not found"

                });
            }

            await timeTable.ExecuteAsync(TableOperation.Delete(timeRecorderEntity));


            string Message = $"Registry with ID {Id}, was successfully deleted :D";

            log.LogInformation(Message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = Message,
                Result = timeRecorderEntity
            });
        }

        [FunctionName(nameof(GetRegistry))]
        public static ActionResult GetRegistry([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Timer/{Id}")] 
            HttpRequest req,[Table("Timer", "Timer", "{Id}", Connection = "AzureWebJobsStorage")] TimeRecorderEntity timeRecorderEntity,
           string Id,ILogger log)
        {
            log.LogInformation("Get a registry");

            if (timeRecorderEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Register found"

                });
            }

            string Message = $"Registry with ID {Id}, was successfully returned :D";

            log.LogInformation(Message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = Message,
                Result = timeRecorderEntity
            });
        }

        [FunctionName(nameof(GetAllRegistries))]
        public static async Task<ActionResult> GetAllRegistries([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Timer")] 
            HttpRequest req,[Table("Timer", Connection = "AzureWebJobsStorage")] CloudTable timeTable,ILogger log)
        {
            log.LogInformation("Get all registries");

            TableQuerySegment<TimeRecorderEntity> Registers =
                await timeTable.ExecuteQuerySegmentedAsync(new TableQuery<TimeRecorderEntity>(), null);

            string Message = $"Registers was successfully retrieved";

            log.LogInformation(Message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = Message,
                Result = Registers
            });
        }

    }
}
