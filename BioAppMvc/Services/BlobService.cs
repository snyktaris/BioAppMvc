using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("AzureStorage:ConnectionString");
        var containerName = configuration.GetValue<string>("AzureStorage:ContainerName");

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var blobClient = _containerClient.GetBlobClient(file.FileName);
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        return blobClient.Uri.ToString(); // returns the public URL for the uploaded file
    }

    public string GetBlobUrl(string fileName)
    {
        return _containerClient.GetBlobClient(fileName).Uri.ToString();
    }
}
