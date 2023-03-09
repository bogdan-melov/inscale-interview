namespace InScale.Queries.InScaleFile.Queries
{
    using Azure.Storage.Sas;
    using FluentResults;
    using InScale.Common.Common.Result;
    using InScale.Common.InScaleFile.Enum;
    using InScale.Contracts.Settings;
    using InScale.Contracts.Storage;
    using InScale.Domain.InScaleFile.Entities;
    using InScale.Queries.Common.Contracts;
    using InScale.Queries.Common.Extensions;
    using InScale.Queries.InScaleFile.Factory;
    using MediatR;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class InScaleFileUriQuery : IRequest<Result<string>>
    {
        public string FileId { get; }

        public string UpdateFromVersion { get; }

        public string Region { get; }

        public string Channel { get; }

        public InScaleFileUriQuery(string fileId, string updateFromVersion, string region, string channel)
        {
            FileId = fileId;
            UpdateFromVersion = updateFromVersion;
            Region = region;
            Channel = channel;
        }
    }

    public class InScaleFileUriQueryHandler : IRequestHandler<InScaleFileUriQuery, Result<string>>
    {
        private readonly IInScaleFileReadOnlyDbContext _inscaleFileDbContext;
        private readonly IStorageService _storageService;
        private readonly IStorageSettings _storageSettings;

        public InScaleFileUriQueryHandler(IInScaleFileReadOnlyDbContext inscaleFileDbContext,
                                          IStorageService storageService,
                                          IStorageSettings storageSettings)
        {
            _inscaleFileDbContext = inscaleFileDbContext;
            _storageService = storageService;
            _storageSettings = storageSettings;
        }

        public async Task<Result<string>> Handle(InScaleFileUriQuery request, CancellationToken cancellationToken)
        {
            Result<Region> regionResult = Region.Create(request.Region);

            if (regionResult.IsFailed)
            {
                return regionResult.ToResult<string>();
            }

            Result<Channel> channelResult = Channel.Create(request.Channel);

            if (channelResult.IsFailed)
            {
                return channelResult.ToResult<string>();
            }

            Result<Domain.InScaleFile.ValueObjects.Version> updateFromVersionResult = Domain.InScaleFile.ValueObjects.Version.Create(request.UpdateFromVersion);

            if (updateFromVersionResult.IsFailed)
            {
                return updateFromVersionResult.ToResult<string>();
            }

            IQueryable<Entities.InScaleFile> inScaleFileQuery =
                _inscaleFileDbContext.GetPartitionedEntities<Entities.InScaleFile>(partitionUid: request.FileId,
                                                                                   predicate: x => x.PreviousVersion == request.UpdateFromVersion);

            Entities.InScaleFile dbInScaleFile = (await inScaleFileQuery.ExecuteQueryAsync()).SingleOrDefault();

            if (dbInScaleFile == null)
            {
                inScaleFileQuery = _inscaleFileDbContext.GetPartitionedEntities<Entities.InScaleFile>(partitionUid: request.FileId,
                                                                                                      predicate: x => x.Version == request.UpdateFromVersion);

                Entities.InScaleFile latestDbInScaleFile = (await inScaleFileQuery.ExecuteQueryAsync()).SingleOrDefault();

                if (latestDbInScaleFile == null)
                {
                    return Result.Fail<string>(ResultErrorCodes.NotFound);
                }

                return Result.Ok("You have hte latest update!");
            }

            if (dbInScaleFile.AvailableFrom > DateTime.UtcNow)
            {
                return Result.Fail<string>(ResultErrorCodes.NotFound);
            }

            Result<InScaleFile> InScalefileResult = dbInScaleFile.ToInScaleFile();

            if (InScalefileResult.IsFailed)
            {
                return InScalefileResult.ToResult<string>();
            }

            if (!InScalefileResult.Value.AvailableInRegion(regionResult.Value))
            {
                return Result.Fail<string>(ResultErrorCodes.FileNotForRegion);
            };

            if (!InScalefileResult.Value.ForChannel(channelResult.Value))
            {
                return Result.Fail<string>(ResultErrorCodes.FileNotForChannel);
            };

            Result<Uri> sasUriResult = await _storageService.GetSasUrlAsync(_storageSettings.ContainerName,
                                                                            InScalefileResult.Value.FilePath,
                                                                            BlobSasPermissions.Read,
                                                                            DateTime.UtcNow.AddHours(1));
            if (sasUriResult.IsFailed)
            {
                return sasUriResult.ToResult<string>();
            }

            return Result.Ok(sasUriResult.Value.AbsoluteUri);
        }
    }
}
