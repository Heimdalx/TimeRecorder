using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace timeRecorder.Test.Helpers
{
    class MockCloudTableTimer : CloudTable
    {
        public MockCloudTableTimer(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableTimer(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableTimer(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.getTimeEntity()
            });
        }

        public override async Task<TableQuerySegment<timearqEntity>> ExecuteQuerySegmentedAsync<timearqEntity>(TableQuery<timearqEntity> query, TableContinuationToken token)
        {
            ConstructorInfo constructor = typeof(TableQuerySegment<timearqEntity>)
                   .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                   .FirstOrDefault(c => c.GetParameters().Count() == 1);

            return await Task.FromResult(constructor.Invoke(new object[] { TestFactory.GetListEntities() }) as TableQuerySegment<timearqEntity>);
        }
    }
}
