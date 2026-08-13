using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ABCRetailManagement.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        private const string ContainerName = "product-images";

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        private async Task<BlobContainerClient> GetContainerAsync()
        {
            var containerClient =
                _blobServiceClient.GetBlobContainerClient(ContainerName);

            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var containerClient = await GetContainerAsync();

            var fileName =
                $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = image.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = image.ContentType
                });

            return fileName;
        }

        public async Task<Stream?> DownloadImageAsync(string fileName)
        {
            var containerClient = await GetContainerAsync();

            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var response = await blobClient.DownloadStreamingAsync();

            return response.Value.Content;
        }

        public async Task DeleteImageAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var containerClient = await GetContainerAsync();
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}