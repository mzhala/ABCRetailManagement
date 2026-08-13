using ABCRetailManagement.Models;
using ABCRetailManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailManagement.Controllers
{
    public class CustomersController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly FileStorageService _fileStorageService;

        public CustomersController(TableStorageService tableStorageService,
            FileStorageService fileStorageService)
        {
            _tableStorageService = tableStorageService;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorageService.GetCustomersAsync();

            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            await _tableStorageService.AddCustomerAsync(customer);

            await _fileStorageService.WriteLogAsync(
                $"Customer {customer.CustomerId} created.");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _tableStorageService.GetCustomerAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            await _tableStorageService.UpdateCustomerAsync(customer);

            await _fileStorageService.WriteLogAsync(
                $"Customer {customer.CustomerId} updated.");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var customer = await _tableStorageService.GetCustomerAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _tableStorageService.DeleteCustomerAsync(id);

            await _fileStorageService.WriteLogAsync(
                $"Customer {id} deleted.");

            return RedirectToAction(nameof(Index));
        }
    }
}