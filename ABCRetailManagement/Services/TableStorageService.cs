using ABCRetailManagement.Models;
using Azure;
using Azure.Data.Tables;

namespace ABCRetailManagement.Services
{
    public class TableStorageService
    {
        private readonly TableServiceClient _tableServiceClient;

        private const string CustomersTableName = "Customers";
        private const string ProductsTableName = "Products";
        private const string OrdersTableName = "Orders";

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

            return customers
                .OrderBy(o => o.Name)
                .ToList();
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

        // Products

        private async Task<TableClient> GetProductsTableAsync()
        {
            var tableClient =
                _tableServiceClient.GetTableClient(ProductsTableName);

            await tableClient.CreateIfNotExistsAsync();

            return tableClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var tableClient = await GetProductsTableAsync();

            var products = new List<Product>();

            await foreach (var entity in tableClient.QueryAsync<ProductEntity>())
            {
                products.Add(new Product
                {
                    ProductId = entity.RowKey,
                    Name = entity.Name,
                    Category = entity.Category,
                    Price = entity.Price,
                    Stock = entity.Stock,
                    ImageName = entity.ImageName
                });
            }

            return products
                .OrderBy(o => o.Name)
                .ToList();
        }

        public async Task<string> AddProductAsync(Product product)
        {
            var tableClient = await GetProductsTableAsync();

            var productId =
                $"PROD-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

            var entity = new ProductEntity
            {
                PartitionKey = "PRODUCT",
                RowKey = productId,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Stock = product.Stock,
                ImageName = product.ImageName ?? string.Empty
            };

            await tableClient.AddEntityAsync(entity);

            return productId;
        }

        public async Task<Product?> GetProductAsync(string productId)
        {
            var tableClient = await GetProductsTableAsync();

            try
            {
                var entity = await tableClient.GetEntityAsync<ProductEntity>(
                    "PRODUCT",
                    productId);

                return new Product
                {
                    ProductId = entity.Value.RowKey,
                    Name = entity.Value.Name,
                    Category = entity.Value.Category,
                    Price = entity.Value.Price,
                    Stock = entity.Value.Stock,
                    ImageName = entity.Value.ImageName
                };
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            var tableClient = await GetProductsTableAsync();

            var entity = new ProductEntity
            {
                PartitionKey = "PRODUCT",
                RowKey = product.ProductId,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Stock = product.Stock,
                ImageName = product.ImageName ?? string.Empty
            };

            await tableClient.UpsertEntityAsync(entity);
        }

        public async Task DeleteProductAsync(string productId)
        {
            var tableClient = await GetProductsTableAsync();

            await tableClient.DeleteEntityAsync(
                "PRODUCT",
                productId);
        }

        //Orders
        private async Task<TableClient> GetOrdersTableAsync()
        {
            var tableClient =
                _tableServiceClient.GetTableClient(OrdersTableName);

            await tableClient.CreateIfNotExistsAsync();

            return tableClient;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var tableClient = await GetOrdersTableAsync();

            var orders = new List<Order>();

            await foreach (var entity in tableClient.QueryAsync<OrderEntity>())
            {
                orders.Add(new Order
                {
                    OrderId = entity.RowKey,
                    CustomerId = entity.CustomerId,
                    ProductId = entity.ProductId,
                    Quantity = entity.Quantity,
                    Status = entity.Status,
                    OrderDate = entity.OrderDate
                });
            }

            return orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public async Task<Order?> GetOrderAsync(string orderId)
        {
            var tableClient = await GetOrdersTableAsync();

            try
            {
                var entity = await tableClient.GetEntityAsync<OrderEntity>(
                    "ORDER",
                    orderId);

                return new Order
                {
                    OrderId = entity.Value.RowKey,
                    CustomerId = entity.Value.CustomerId,
                    ProductId = entity.Value.ProductId,
                    Quantity = entity.Value.Quantity,
                    Status = entity.Value.Status,
                    OrderDate = entity.Value.OrderDate
                };
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task<string> AddOrderAsync(Order order)
        {
            var tableClient = await GetOrdersTableAsync();

            var orderId =
                $"ORD-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

            var entity = new OrderEntity
            {
                PartitionKey = "ORDER",
                RowKey = orderId,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                Status = "Pending",
                OrderDate = DateTime.UtcNow
            };

            await tableClient.AddEntityAsync(entity);

            return orderId;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var tableClient = await GetOrdersTableAsync();

            var entity = new OrderEntity
            {
                PartitionKey = "ORDER",
                RowKey = order.OrderId,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                Status = order.Status,
                OrderDate = order.OrderDate
            };

            await tableClient.UpsertEntityAsync(entity);
        }

        public async Task DeleteOrderAsync(string orderId)
        {
            var tableClient = await GetOrdersTableAsync();

            await tableClient.DeleteEntityAsync(
                "ORDER",
                orderId);
        }


    }
}