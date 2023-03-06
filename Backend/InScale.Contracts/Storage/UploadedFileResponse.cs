namespace InScale.Contracts.Storage
{
    using System;

    public class UploadedFileResponseDto
    {
        public Uri FileUri { get; }

        public string FileId { get; }

        public UploadedFileResponseDto(Uri fileUri, string fileId)
        {
            FileUri = fileUri;
            FileId = fileId;
        }
    }
}
