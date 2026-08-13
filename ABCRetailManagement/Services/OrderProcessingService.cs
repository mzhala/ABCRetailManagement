using System.Text.Json;

namespace ABCRetailManagement.Services
{
    public class OrderProcessingService
    {
        private readonly QueueStorageService _queueStorageService;
        private readonly TableStorageService _tableStorageService;
        private readonly FileStorageService _fileStorageService;

        public OrderProcessingService(
            QueueStorageService queueStorageService,
            TableStorageService tableStorageService,
            FileStorageService fileStorageService
            )
        {
            _queueStorageService = queueStorageService;
            _tableStorageService = tableStorageService;
            _fileStorageService = fileStorageService;
        }

        public async Task ProcessNextOrderAsync()
        {
            var message =
                await _queueStorageService.GetOrderMessageAsync();

            if (message == null)
            {
                return;
            }

            var orderMessage =
                JsonSerializer.Deserialize<OrderQueueMessage>(
                    message.Value.Message);

            if (orderMessage == null)
            {
                await _queueStorageService.DeleteMessageAsync(
                    message.Value.MessageId,
                    message.Value.PopReceipt);

                return;
            }

            var order =
                await _tableStorageService.GetOrderAsync(
                    orderMessage.OrderId);

            if (order == null)
            {
                await _queueStorageService.DeleteMessageAsync(
                    message.Value.MessageId,
                    message.Value.PopReceipt);

                return;
            }

            var product =
                await _tableStorageService.GetProductAsync(
                    order.ProductId);

            if (product == null)
            {
                order.Status = "Failed";

                await _tableStorageService.UpdateOrderAsync(order);

                await _queueStorageService.DeleteMessageAsync(
                    message.Value.MessageId,
                    message.Value.PopReceipt);

                return;
            }

            if (product.Stock < order.Quantity)
            {
                order.Status = "Failed";

                await _tableStorageService.UpdateOrderAsync(order);

                await _fileStorageService.WriteLogAsync(
                    $"Order {order.OrderId} failed because there was insufficient stock.");

                await _queueStorageService.DeleteMessageAsync(
                    message.Value.MessageId,
                    message.Value.PopReceipt);

                return;
            }

            product.Stock -= order.Quantity;

            await _tableStorageService.UpdateProductAsync(product);

            order.Status = "Completed";

            await _tableStorageService.UpdateOrderAsync(order);

            await _fileStorageService.WriteLogAsync(
                $"Order {order.OrderId} processed successfully.");

            await _queueStorageService.DeleteMessageAsync(
                message.Value.MessageId,
                message.Value.PopReceipt);
        }
    }

    public class OrderQueueMessage
    {
        public string OrderId { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public string ProductId { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}