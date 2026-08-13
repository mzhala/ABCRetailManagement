using Azure.Storage.Queues;
using System.Text.Json;

namespace ABCRetailManagement.Services
{
    public class QueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;

        private const string QueueName = "order-processing";

        public QueueStorageService(QueueServiceClient queueServiceClient)
        {
            _queueServiceClient = queueServiceClient;
        }

        private async Task<QueueClient> GetQueueAsync()
        {
            var queueClient =
                _queueServiceClient.GetQueueClient(QueueName);

            await queueClient.CreateIfNotExistsAsync();

            return queueClient;
        }

        public async Task SendOrderMessageAsync(
            string orderId,
            string customerId,
            string productId,
            int quantity)
        {
            var queueClient = await GetQueueAsync();

            var message = new
            {
                OrderId = orderId,
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity
            };

            var json = JsonSerializer.Serialize(message);

            await queueClient.SendMessageAsync(json);
        }

        // Retrieves one waiting message.
        public async Task<(string MessageId, string PopReceipt, string Message)?> GetOrderMessageAsync()
        {
            var queueClient = await GetQueueAsync();

            var response = await queueClient.ReceiveMessageAsync();

            if (response.Value == null)
            {
                return null;
            }

            return (
                response.Value.MessageId,
                response.Value.PopReceipt,
                response.Value.MessageText
            );
        }

        // Remove a processed message
        public async Task DeleteMessageAsync(
            string messageId,
            string popReceipt)
         {
            var queueClient = await GetQueueAsync();

            await queueClient.DeleteMessageAsync(
                messageId,
                popReceipt);
        }
    }
}