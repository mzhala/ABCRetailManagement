using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetailManagement.Services
{
    public class FileStorageService
    {
        private readonly ShareServiceClient _shareServiceClient;

        private const string ShareName = "application-logs";

        public FileStorageService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
        }

        private async Task<ShareClient> GetShareAsync()
        {
            var shareClient =
                _shareServiceClient.GetShareClient(ShareName);

            await shareClient.CreateIfNotExistsAsync();

            return shareClient;
        }

        public async Task WriteLogAsync(string message)
        {
            var shareClient = await GetShareAsync();

            var directoryClient =
                shareClient.GetDirectoryClient("logs");

            await directoryClient.CreateIfNotExistsAsync();

            var fileName =
                $"log-{DateTime.UtcNow:yyyyMMdd}.txt";

            var fileClient =
                directoryClient.GetFileClient(fileName);

            var existingContent = string.Empty;

            if (await fileClient.ExistsAsync())
            {
                var download =
                    await fileClient.DownloadAsync();

                using var reader =
                    new StreamReader(download.Value.Content);

                existingContent =
                    await reader.ReadToEndAsync();
            }

            var newContent =
                $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";

            var content =
                existingContent + newContent;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(content));

            await fileClient.CreateAsync(stream.Length);

            await fileClient.UploadAsync(
                stream);
        }

        public async Task<List<string>> GetLogFilesAsync()
        {
            var shareClient = await GetShareAsync();

            var directoryClient =
                shareClient.GetDirectoryClient("logs");

            var files = new List<string>();

            await foreach (var item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                {
                    files.Add(item.Name);
                }
            }

            return files
                .OrderByDescending(f => f)
                .ToList();
        }

        public async Task<string?> ReadLogAsync(string fileName)
        {
            var shareClient = await GetShareAsync();

            var directoryClient =
                shareClient.GetDirectoryClient("logs");

            var fileClient =
                directoryClient.GetFileClient(fileName);

            if (!await fileClient.ExistsAsync())
            {
                return null;
            }

            var response =
                await fileClient.DownloadAsync();

            using var reader =
                new StreamReader(response.Value.Content);

            return await reader.ReadToEndAsync();
        }

        public async Task<Stream?> DownloadLogAsync(string fileName)
        {
            var shareClient = await GetShareAsync();

            var directoryClient =
                shareClient.GetDirectoryClient("logs");

            var fileClient =
                directoryClient.GetFileClient(fileName);

            if (!await fileClient.ExistsAsync())
            {
                return null;
            }

            var response =
                await fileClient.DownloadAsync();

            var memoryStream = new MemoryStream();

            await response.Value.Content.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}