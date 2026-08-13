using ABCRetailManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailManagement.Controllers
{
    public class LogsController : Controller
    {
        private readonly FileStorageService _fileStorageService;

        public LogsController(
            FileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var files =
                await _fileStorageService.GetLogFilesAsync();

            return View(files);
        }

        public async Task<IActionResult> ViewLog(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            if (id.Contains("/") || id.Contains("\\"))
            {
                return BadRequest();
            }

            var content =
                await _fileStorageService.ReadLogAsync(id);

            if (content == null)
            {
                return NotFound();
            }

            ViewBag.FileName = id;

            return View("ViewLog", content);
        }

        public async Task<IActionResult> Download(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            if (id.Contains("/") || id.Contains("\\"))
            {
                return BadRequest();
            }

            var file = await _fileStorageService.DownloadLogAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            return File(
                file,
                "text/plain",
                id);
        }
    }
}