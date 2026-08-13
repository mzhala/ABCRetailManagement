using Azure;
using Azure.Data.Tables;

namespace ABCRetailManagement.Models
{
    public class OrderEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;

        public string RowKey { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public string ProductId { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}