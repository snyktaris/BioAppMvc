using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BioAppMvc.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("DefaultEndpointsProtocol=https;AccountName=multisstorage;AccountKey=Jkume2ylRW9cUsKKvDVESnjrzIgMoMcQetu6xhEDYUc3vMDMGuIEzHaOmnzf3DVQOQEC32GnYt/6+AStvX5U2Q==;EndpointSuffix=core.windows.net");
            var containerName = configuration["AzureStorage:ContainerName"];

            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);  // Ensures the container is created if it doesn't exist
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            // Upload the file and overwrite if it exists
            await blobClient.UploadAsync(fileStream, overwrite: true);
            // Set content type
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });

            // Return the URL of the uploaded file
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
}
