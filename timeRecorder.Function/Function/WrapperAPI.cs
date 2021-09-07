using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using timeRecorder.Function.Entities;
using System.Collections.Generic;
using timeRecorder.Common.Responses;

namespace timeRecorder.Function.Function
{
    public static class WrapperAPI
    {
        [FunctionName(nameof(GetWrapperByDate))]
        public static async Task<IActionResult> GetWrapperByDate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidate/{date}")] HttpRequest req,
            [Table("WrapeTable", Connection = "AzureWebJobsStorage")] CloudTable wrappeTable,string date,ILogger log)
        {
            log.LogInformation($"Returning all wrapes registries by day {date}.");

            DateTime startDay = DateTime.Parse(date + " 00:00 ");
            DateTime endDay = DateTime.Parse(date + " 23:59 ");
            TableQuerySegment<WrapeEntity> consolidates = await wrappeTable.ExecuteQuerySegmentedAsync(new TableQuery<WrapeEntity>(), null);
            List<WrapeEntity> wrapeList = new List<WrapeEntity>();

            foreach (WrapeEntity wrapeRegistries in consolidates)
            {
                if (wrapeRegistries.Date >= startDay && wrapeRegistries.Date <= endDay)
                {
                    wrapeList.Add(wrapeRegistries);
                }
            }


            string message = $"Wrape Registries by date: {date} returned";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = wrapeList
            });
        }
    }
}
