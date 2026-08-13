using Azure;
using Azure.Data.Tables;

namespace ABCRetailManagement.Models
{
    public class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;

        public string RowKey { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public double Price { get; set; }

        public int Stock { get; set; }

        public string ImageName { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }        
    }
}