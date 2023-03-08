namespace InScale.Contracts.Storage
{
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;
    using Azure.Storage.Sas;
    using FluentResults;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AzureStorageService : IStorageService
    {
        private readonly BlobServiceClient _client;

        public AzureStorageService(BlobServiceClient client)
        {
            _client = client;
        }

        public async Task<Result<Uri>> GetSasUrlAsync(string containerId, string fileId, BlobSasPermissions permission, DateTime expiresOn)
        {
            try
            {
                if (containerId == null) { throw new ArgumentException($"You must provide {nameof(containerId)}"); }
                if (fileId == null) { throw new ArgumentException($"You must provide {nameof(containerId)}"); }
                if (expiresOn < DateTime.UtcNow)
                {
                    throw new ArgumentException($"{nameof(expiresOn)} must reference a moment in the future.");
                }

                BlobContainerClient storageContainer = await GetContainerAsync(containerId);

                BlobClient blobClient = storageContainer.GetBlobClient(fileId);

                var sasBuilder = new BlobSasBuilder(permissions: permission, expiresOn: expiresOn)
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

                return Result.Ok(sasUri);
            }
            catch (Exception ex)
            {
                return Result.Fail<Uri>(ex.Message);
            }
        }

        public async Task<Result<UploadedFileResponseDto>> UploadAsync(
            string containerId,
            string fileId,
            byte[] content,
            string contentType)
        {
            try
            {
                if (content.Length == 0) { throw new ArgumentException($"You must provide {nameof(content)}"); }
                if (containerId == null) { throw new ArgumentException($"You must provide {nameof(containerId)}"); }
                if (fileId == null) { throw new ArgumentException($"You must provide {nameof(containerId)}"); }
                if (contentType == null) { throw new ArgumentException($"You must provide {nameof(contentType)}"); }

                BlobContainerClient storageContainer = await GetContainerAsync(containerId);

                IEnumerable<char> invalidChars = Path.GetInvalidPathChars();
                invalidChars = invalidChars.Union(Path.GetInvalidFileNameChars());
                invalidChars = invalidChars.Where(x => x != '/');

                string readyFileId = string.Concat(fileId.Split(invalidChars.ToArray()));

                BlobClient blobClient = storageContainer.GetBlobClient(fileId);

                using (Stream fileStream = new MemoryStream(content))
                {
                    await blobClient.UploadAsync(content: fileStream, overwrite: true);
                }

                await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });

                return Result.Ok(new UploadedFileResponseDto(blobClient.Uri, fileId));
            }
            catch (Exception ex)
            {
                return Result.Fail<UploadedFileResponseDto>(ex.Message);
            }
        }

        private async Task<BlobContainerClient> GetContainerAsync(string containerId)
        {
            BlobContainerClient containerClient = _client.GetBlobContainerClient(containerId);

            Response<bool> containerExists = await containerClient.ExistsAsync();
            if (containerExists.Value)
            {
                return containerClient;
            }
            else
            {
                var newContainer = await _client.CreateBlobContainerAsync(containerId);
                return newContainer.Value;
            }
        }
    }
}
