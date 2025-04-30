using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
        var containerName = configuration["AzureStorage:ContainerName"];

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true);
        await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var downloadInfo = await blobClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }

    public async Task<bool> DeleteFileAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<List<string>> ListFilesAsync()
    {
        var fileList = new List<string>();

        await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync())
        {
            var blobClient = _containerClient.GetBlobClient(blobItem.Name);
            fileList.Add(blobClient.Uri.ToString());
        }

        return fileList;
    }
}
