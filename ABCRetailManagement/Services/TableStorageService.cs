using ABCRetailManagement.Models;
using Azure;
using Azure.Data.Tables;

namespace ABCRetailManagement.Services
{
    public class TableStorageService
    {
        private readonly TableServiceClient _tableServiceClient;

        private const string CustomersTableName = "Customers";

        public TableStorageService(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        private async Task<TableClient> GetCustomersTableAsync()
        {
            var tableClient =
                _tableServiceClient.GetTableClient(CustomersTableName);

            await tableClient.CreateIfNotExistsAsync();

            return tableClient;
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            var tableClient = await GetCustomersTableAsync();

            var customers = new List<Customer>();

            await foreach (var entity in tableClient.QueryAsync<CustomerEntity>())
            {
                customers.Add(new Customer
                {
                    CustomerId = entity.RowKey,
                    Name = entity.Name,
                    Email = entity.Email,
                    Phone = entity.Phone,
                    Location = entity.Location
                });
            }

            return customers;
        }

        public async Task<string> AddCustomerAsync(Customer customer)
        {
            var tableClient = await GetCustomersTableAsync();

            var customerId =
                $"CUST-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

            var entity = new CustomerEntity
            {
                PartitionKey = "CUSTOMER",
                RowKey = customerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Location = customer.Location
            };

            await tableClient.AddEntityAsync(entity);

            return customerId;
        }

        public async Task<Customer?> GetCustomerAsync(string customerId)
        {
            var tableClient = await GetCustomersTableAsync();

            try
            {
                var entity = await tableClient.GetEntityAsync<CustomerEntity>(
                    "CUSTOMER",
                    customerId);

                return new Customer
                {
                    CustomerId = entity.Value.RowKey,
                    Name = entity.Value.Name,
                    Email = entity.Value.Email,
                    Phone = entity.Value.Phone,
                    Location = entity.Value.Location
                };
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var tableClient = await GetCustomersTableAsync();

            var entity = new CustomerEntity
            {
                PartitionKey = "CUSTOMER",
                RowKey = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Location = customer.Location
            };

            await tableClient.UpsertEntityAsync(entity);
        }

        public async Task DeleteCustomerAsync(string customerId)
        {
            var tableClient = await GetCustomersTableAsync();

            await tableClient.DeleteEntityAsync(
                "CUSTOMER",
                customerId);
        }
    }
}