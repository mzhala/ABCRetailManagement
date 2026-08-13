using Azure.Data.Tables;

namespace ABCRetailManagement.Services
{
    public class TableStorageService
    {
        private readonly TableServiceClient _tableServiceClient;

        public TableStorageService(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        public async Task CreateTableAsync(string tableName)
        {
            await _tableServiceClient.CreateTableIfNotExistsAsync(tableName);
        }
    }
}