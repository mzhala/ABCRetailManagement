using ABCRetailManagement.Models;
using ABCRetailManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueStorageService _queueStorageService;
        private readonly OrderProcessingService _orderProcessingService;
        private readonly FileStorageService _fileStorageService;

        public OrdersController(
            TableStorageService tableStorageService,
            QueueStorageService queueStorageService,
            OrderProcessingService orderProcessingService,
            FileStorageService fileStorageService
            )
        {
            _tableStorageService = tableStorageService;
            _queueStorageService = queueStorageService;
            _orderProcessingService = orderProcessingService;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetOrdersAsync();

            await LoadOrderOptions();

            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            await LoadOrderOptions();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                await LoadOrderOptions();

                return View(order);
            }

            try
            {
                var orderId =
                    await _tableStorageService.AddOrderAsync(order);

                await _queueStorageService.SendOrderMessageAsync(
                    orderId,
                    order.CustomerId,
                    order.ProductId,
                    order.Quantity);

                await _fileStorageService.WriteLogAsync(
                    $"Order {orderId} created.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The order could not be created. Please try again.");

                await LoadOrderOptions();

                return View(order);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var order = await _tableStorageService.GetOrderAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            await LoadOrderOptions();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            if (!ModelState.IsValid)
            {
                await LoadOrderOptions();
                return View(order);
            }

            try
            {
                var existingOrder =
                    await _tableStorageService.GetOrderAsync(order.OrderId);

                if (existingOrder == null)
                {
                    return NotFound();
                }

                existingOrder.CustomerId = order.CustomerId;

                await _tableStorageService.UpdateOrderAsync(existingOrder);

                await _fileStorageService.WriteLogAsync(
                    $"Order {order.OrderId} updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The order could not be updated. Please try again.");

                await LoadOrderOptions();

                return View(order);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            var order = await _tableStorageService.GetOrderAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _tableStorageService.DeleteOrderAsync(id);

                await _fileStorageService.WriteLogAsync(
                    $"Order {id} deleted.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The order could not be deleted. Please try again.");

                var order =
                    await _tableStorageService.GetOrderAsync(id);

                if (order == null)
                {
                    return NotFound();
                }

                return View("Delete", order);
            }
        }

        // Process the next order in the queue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessNext()
        {
            await _orderProcessingService.ProcessNextOrderAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadOrderOptions()
        {
            ViewBag.Customers =
                await _tableStorageService.GetCustomersAsync();

            ViewBag.Products =
                await _tableStorageService.GetProductsAsync();
        }
    }
}