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

        public OrdersController(
            TableStorageService tableStorageService,
            QueueStorageService queueStorageService,
            OrderProcessingService orderProcessingService)
        {
            _tableStorageService = tableStorageService;
            _queueStorageService = queueStorageService;
            _orderProcessingService = orderProcessingService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetOrdersAsync();

            return View(orders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
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

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The order could not be created. Please try again.");

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

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            if (!ModelState.IsValid)
            {
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
                existingOrder.ProductId = order.ProductId;
                existingOrder.Quantity = order.Quantity;
                existingOrder.Status = order.Status;

                await _tableStorageService.UpdateOrderAsync(existingOrder);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The order could not be updated. Please try again.");

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
    }
}