namespace InScale.Commands.InScaleFile.Commands
{
    using FluentResults;
    using InScale.Common.Common.Result;
    using InScale.Contracts.InScaleFile.Repositories;
    using InScale.Contracts.Settings;
    using InScale.Contracts.Storage;
    using InScale.Domain.InScaleFile.Entities;
    using InScale.Domain.InScaleFile.Extensions;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class AddInScaleFileCommand : IRequest<Result<InScaleFile>>
    {
        public IFormFile File { get; }

        public string Version { get; }

        public List<string> AvailableInRegions { get; }

        public DateTime AvailableFrom { get; }

        public List<string> Channels { get; }

        public AddInScaleFileCommand(IFormFile file,
                                     string version,
                                     List<string> availableInRegions,
                                     DateTime availableFrom,
                                     List<string> channels)
        {
            File = file;
            Version = version;
            AvailableInRegions = availableInRegions;
            AvailableFrom = availableFrom;
            Channels = channels;
        }
    }

    public class AddInScaleFileCommandHandler : IRequestHandler<AddInScaleFileCommand, Result<InScaleFile>>
    {
        private readonly IInScaleFileRepository _inScaleFileRepository;
        private readonly IStorageService _storageService;
        private readonly IStorageSettings _storageSettings;


        public AddInScaleFileCommandHandler(IInScaleFileRepository inScaleFileRepository, IStorageService storageService, IStorageSettings storageSettings)
        {
            _inScaleFileRepository = inScaleFileRepository;
            _storageService = storageService;
            _storageSettings = storageSettings;
        }

        public async Task<Result<InScaleFile>> Handle(AddInScaleFileCommand request, CancellationToken cancellationToken)
        {
            MemoryStream sream = new MemoryStream();
            await request.File.CopyToAsync(sream);
            byte[] fileBytes = sream.ToArray();
            string fileId = request.File.FileName.Split('.')[0].ToLower();

            Result<List<InScaleFile>> inScaleFilesResult = await _inScaleFileRepository.GetInScaleFilesByFileId(fileId);

            if (inScaleFilesResult.IsFailed)
            {
                return Result.Fail<InScaleFile>(inScaleFilesResult.Errors);
            }

            Result<InScaleFile> newInScaleFileResult;

            if (!inScaleFilesResult.Value.Any())
            {
                newInScaleFileResult = InScaleFile.Create(uid: Guid.NewGuid(),
                                                          createdOn: DateTime.UtcNow,
                                                          fileId: fileId,
                                                          previousVersion: null,
                                                          version: request.Version,
                                                          filePath: null,
                                                          availableInRegions: request.AvailableInRegions,
                                                          availableFrom: request.AvailableFrom,
                                                          channels: request.Channels);
                if (newInScaleFileResult.IsFailed)
                {
                    return newInScaleFileResult;
                }

            }
            else
            {
                List<InScaleFile> sortedFilesByVersion = inScaleFilesResult.Value.SortByVersion();

                InScaleFile fileWithLatestVersion = sortedFilesByVersion.Last();

                newInScaleFileResult = InScaleFile.Create(uid: Guid.NewGuid(),
                                                          createdOn: DateTime.UtcNow,
                                                          fileId: fileId,
                                                          previousVersion: fileWithLatestVersion.Version.ToString(),
                                                          version: request.Version,
                                                          filePath: null,
                                                          availableInRegions: request.AvailableInRegions,
                                                          availableFrom: request.AvailableFrom,
                                                          channels: request.Channels);
                if (newInScaleFileResult.IsFailed)
                {
                    return newInScaleFileResult;
                }

                if (newInScaleFileResult.Value.PreviousVersion.IsUpperVersionOf(newInScaleFileResult.Value.Version))
                {
                    return Result.Fail<InScaleFile>(ResultErrorCodes.VersionNotValid);
                }
            }

            string filePath = $"{fileId}/{request.Version}/{request.File.FileName}";

            Result<UploadedFileResponseDto> uploadedFileResult = await _storageService.UploadAsync(_storageSettings.ContainerName,
                                                                                                   filePath,
                                                                                                   fileBytes,
                                                                                                   request.File.ContentType);
            if (uploadedFileResult.IsFailed)
            {
                return Result.Fail(uploadedFileResult.Errors);
            }

            newInScaleFileResult.Value.AddFilePath(uploadedFileResult.Value.FileId);

            Result insertInScaleFileResult = await _inScaleFileRepository.SaveInScaleFile(newInScaleFileResult.Value);

            if (insertInScaleFileResult.IsFailed)
            {
                return Result.Fail<InScaleFile>(insertInScaleFileResult.Errors);
            }

            return newInScaleFileResult;
        }
    }
}
