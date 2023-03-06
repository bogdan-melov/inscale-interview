namespace InScale.Contracts.Storage
{
    using FluentResults;
    using System;
    using System.Threading.Tasks;

    public interface IStorageService
    {
        Task<Result<UploadedFileResponseDto>> UploadAsync(string containerId, string fileId, byte[] content, string contentType, bool overwriteIfExists);

        Task<Result<Uri>> GetSasUrlForDownloadAsync(string containerId, string fileId, DateTime expiresOn);
    }
}
