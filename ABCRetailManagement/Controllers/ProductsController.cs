using ABCRetailManagement.Models;
using ABCRetailManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailManagement.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly BlobStorageService _blobStorageService;
        private readonly FileStorageService _fileStorageService;

        public ProductsController(
            TableStorageService tableStorageService,
            BlobStorageService blobStorageService,
            FileStorageService fileStorageService
            )
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetProductsAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                if (image != null && image.Length > 0)
                {
                    product.ImageName =
                        await _blobStorageService.UploadImageAsync(image);
                }

                await _tableStorageService.AddProductAsync(product);

                await _fileStorageService.WriteLogAsync(
                    $"Product {product.ProductId} created.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The product could not be added. Please try again.");

                return View(product);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var product = await _tableStorageService.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                if (image != null && image.Length > 0)
                {
                    product.ImageName =
                        await _blobStorageService.UploadImageAsync(image);
                }

                await _tableStorageService.UpdateProductAsync(product);

                await _fileStorageService.WriteLogAsync(
                    $"Product {product.ProductId} updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The product could not be updated. Please try again.");

                return View(product);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            var product = await _tableStorageService.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _tableStorageService.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                if (!string.IsNullOrEmpty(product.ImageName))
                {
                    await _blobStorageService.DeleteImageAsync(product.ImageName);
                }

                await _tableStorageService.DeleteProductAsync(id);

                await _fileStorageService.WriteLogAsync(
                    $"Product {id} deleted.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "The product could not be deleted. Please try again.");

                return View("Delete", product);
            }
        }

        public async Task<IActionResult> Image(string fileName)
        {
            var image = await _blobStorageService.DownloadImageAsync(fileName);

            if (image == null)
            {
                return NotFound();
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return File(image, contentType);
        }
    }
}