namespace InScale.Contracts.Storage
{
    using Azure.Storage.Sas;
    using FluentResults;
    using System;
    using System.Threading.Tasks;

    public interface IStorageService
    {
        Task<Result<UploadedFileResponseDto>> UploadAsync(string containerId, string fileId, byte[] content, string contentType);

        Task<Result<Uri>> GetSasUrlAsync(string containerId, string fileId, BlobSasPermissions permission, DateTime expiresOn);
    }
}
