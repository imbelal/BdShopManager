namespace Domain.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string containerName = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string fileName, string containerName = null, CancellationToken cancellationToken = default);
        Task<Stream> DownloadFileAsync(string fileName, string containerName = null, CancellationToken cancellationToken = default);
        string GetFileUrl(string fileName, string containerName = null);
        string GetDefaultContainerName();
    }
} 