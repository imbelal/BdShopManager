using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Interfaces;
using Microsoft.Extensions.Options;
using Application.Services.Common;

namespace Infrastructure.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly AzureStorageSettings _azureStorageSettings;

        public AzureBlobStorageService(IOptions<AppSettings> appSettings)
        {
            _azureStorageSettings = appSettings.Value.AzureStorage;
            _blobServiceClient = new BlobServiceClient(_azureStorageSettings.ConnectionString);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string containerName = null, CancellationToken cancellationToken = default)
        {
            var targetContainerName = containerName ?? _azureStorageSettings.ProductPhotoContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(targetContainerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobClient = containerClient.GetBlobClient(fileName);
            
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(fileStream, blobHttpHeaders, cancellationToken: cancellationToken);

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteFileAsync(string fileName, string containerName = null, CancellationToken cancellationToken = default)
        {
            var targetContainerName = containerName ?? _azureStorageSettings.ProductPhotoContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(targetContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            return response.Value;
        }

        public async Task<Stream> DownloadFileAsync(string fileName, string containerName = null, CancellationToken cancellationToken = default)
        {
            var targetContainerName = containerName ?? _azureStorageSettings.ProductPhotoContainer;
            var containerClient = _blobServiceClient.GetBlobContainerClient(targetContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.DownloadAsync(cancellationToken: cancellationToken);
            return response.Value.Content;
        }

        public string GetFileUrl(string fileName, string containerName = null)
        {
            var targetContainerName = containerName ?? _azureStorageSettings.ProductPhotoContainer;
            
            if (string.IsNullOrEmpty(_azureStorageSettings.BaseUrl))
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(targetContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                return blobClient.Uri.ToString();
            }

            return $"{_azureStorageSettings.BaseUrl.TrimEnd('/')}/{targetContainerName}/{fileName}";
        }

        public string GetDefaultContainerName()
        {
            return _azureStorageSettings.ProductPhotoContainer;
        }
    }
} 