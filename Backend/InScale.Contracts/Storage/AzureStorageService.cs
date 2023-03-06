namespace InScale.Contracts.Storage
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;
    using Azure.Storage.Sas;
    using FluentResults;
    using InScale.Contracts.Exceptions;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AzureStorageService : IStorageService
    {
        private readonly BlobServiceClient _client;
        private readonly ILogger<AzureStorageService> _logger;

        public AzureStorageService(
            BlobServiceClient client,
            ILogger<AzureStorageService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<Result<Uri>> GetSasUrlForDownloadAsync(string containerId, string fileId, DateTime expiresOn)
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

                if (!(await blobClient.ExistsAsync())) { throw new FileNotFoundException($"File with id {fileId} does not exist"); }

                var sasBuilder = new BlobSasBuilder(permissions: BlobSasPermissions.Read, expiresOn: expiresOn)
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

                return sasUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Azure storage service failed for {containerId}, {fileId} and {expiresOn}");

                return Result.Fail<Uri>(ResultErrorCodes.InternalServerError);
            }
        }

        public async Task<Result<UploadedFileResponseDto>> UploadAsync(
            string containerId,
            string fileId,
            byte[] content,
            string contentType,
            bool overwriteIfExists)
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
                    await blobClient.UploadAsync(content: fileStream, overwrite: overwriteIfExists);
                }

                await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });

                return Result.Ok(new UploadedFileResponseDto(blobClient.Uri, fileId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Azure storage service failed for {containerId}, {fileId} and {contentType}");
                return Result.Fail<UploadedFileResponseDto>(ResultErrorCodes.InternalServerError);
            }
        }

        private async Task<BlobContainerClient> GetContainerAsync(string containerId)
        {
            BlobContainerClient containerClient = _client.GetBlobContainerClient(containerId);

            if (await containerClient.ExistsAsync())
            {
                return containerClient;
            }
            else
            {
                throw new DirectoryNotFoundException($"Container with {nameof(containerId)} {containerId} doen't exist");
            }
        }
    }
}
