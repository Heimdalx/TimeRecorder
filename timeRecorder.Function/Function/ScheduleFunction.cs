using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using timeRecorder.Function.Entities;

namespace timeRecorder.Function.Function
{
    public static class ScheduleFunction
    {
        [FunctionName("ScheduleFunction")]
        public static async Task Execute([TimerTrigger("*/1 * * * *")] TimerInfo myTimer,
           [Table("Timer", Connection = "AzureWebJobsStorage")] CloudTable timer,
           [Table("WrapeTable", Connection = "AzureWebJobsStorage")] CloudTable wrapeTable,
           ILogger log)
        {
            log.LogInformation($"Wrapping data in table wrape");

            string filter = TableQuery.GenerateFilterConditionForBool("WrapRegistries", QueryComparisons.Equal, false);
            TableQuery<TimeRecorderEntity> query = new TableQuery<TimeRecorderEntity>().Where(filter);
            TableQuerySegment<TimeRecorderEntity> unwrapedRegistries = await timer.ExecuteQuerySegmentedAsync(query, null);
            int totalAdded = 0;
            int totalUpdated = 0;

            //Set by id
            List<IGrouping<int, TimeRecorderEntity>> unwrapeGroup = unwrapedRegistries.GroupBy(timer => timer.IdEmployee).OrderBy(order => order.Key).ToList();
            foreach (IGrouping<int, TimeRecorderEntity> group in unwrapeGroup)
            {
                TimeSpan diff;
                double totalMins = 0;
                List<TimeRecorderEntity> orderedRegistries = group.OrderBy(order => order.Registry).ToList();
                int isEven = orderedRegistries.Count % 2 == 0 ? orderedRegistries.Count : orderedRegistries.Count - 1;
                TimeRecorderEntity[] timerAux = orderedRegistries.ToArray();
                try
                {
                    for (int i = 0; i < isEven; i++)
                    {
                        await ChangeConsolidateStatus(timerAux[i].RowKey, timer);
                        if (i % 2 != 0 && timerAux.Length > 1)
                        {
                            diff = timerAux[i].Registry - timerAux[i - 1].Registry;
                            totalMins += diff.TotalMinutes;
                            TableQuerySegment<WrapeEntity> allConsolidated = await wrapeTable.ExecuteQuerySegmentedAsync(new TableQuery<WrapeEntity>(), null);
                            IEnumerable<WrapeEntity> existConsolidated = allConsolidated.Where(employee => employee.IdEmployee == timerAux[i].IdEmployee);
                            if (existConsolidated == null || existConsolidated.Count() == 0)
                            {
                                WrapeEntity wrapeRegistries = new WrapeEntity
                                {
                                    IdEmployee = timerAux[i].IdEmployee,
                                    Date = DateTime.Today,
                                    MinsDone = (int)totalMins,
                                    ETag = "*",
                                    PartitionKey = "WrapeTable",
                                    RowKey = timerAux[i].RowKey
                                };
                                TableOperation addWrapedOperation = TableOperation.Insert(wrapeRegistries);
                                await wrapeTable.ExecuteAsync(addWrapedOperation);
                                totalAdded++;
                            }
                            else
                            {
                                TableOperation findOp = TableOperation.Retrieve<WrapeEntity>("WrapeTable", existConsolidated.First().RowKey);
                                TableResult findRes = await wrapeTable.ExecuteAsync(findOp);
                                WrapeEntity consolidatedEntity = (WrapeEntity)findRes.Result;
                                consolidatedEntity.Date = existConsolidated.First().Date;
                                consolidatedEntity.MinsDone += (int)totalMins;
                                TableOperation addConsolidatedOperation = TableOperation.Replace(consolidatedEntity);
                                await wrapeTable.ExecuteAsync(addConsolidatedOperation);
                                totalUpdated++;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    string errorMessage = error.Message;
                    throw;
                }
            }
            log.LogInformation($"¡Well done!. \n  New data saved was: {totalAdded} \n Data updated was: {totalUpdated}.");

        }

        private static async Task ChangeConsolidateStatus(string id, CloudTable timer)
        {
            TableOperation findOperation = TableOperation.Retrieve<TimeRecorderEntity>("timer", id);
            TableResult findResult = await timer.ExecuteAsync(findOperation);
            TimeRecorderEntity timeRecorderEntity = (TimeRecorderEntity)findResult.Result;
            timeRecorderEntity.WrapRegistries = true;
            TableOperation addOperation = TableOperation.Replace(timeRecorderEntity);
            await timer.ExecuteAsync(addOperation);
        }
    }
}
