namespace InScale.Domain.InScaleFile.Entities
{
    using FluentResults;
    using InScale.Common.InScaleFile.Enum;
    using InScale.Domain.Common;
    using InScale.Domain.InScaleFile.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InScaleFile : Entity
    {
        public string FileId;

        public ValueObjects.Version PreviousVersion;

        public ValueObjects.Version Version;

        public string FilePath;

        public List<Region> AvailableInRegions;

        public DateTime AvailableFrom;

        public List<Channel> Channels;

        private InScaleFile(Guid uid,
                           DateTime createdOn,
                           string fileId,
                           ValueObjects.Version previousVersion,
                           ValueObjects.Version version,
                           string filePath,
                           List<Region> availableInRegions,
                           DateTime availableFrom,
                           List<Channel> channels) : base(uid, createdOn)
        {
            FileId = fileId;
            PreviousVersion = previousVersion;
            Version = version;
            FilePath = filePath;
            AvailableInRegions = availableInRegions;
            AvailableFrom = availableFrom;
            Channels = channels;
        }

        public static Result<InScaleFile> Create(Guid uid,
                                                  DateTime createdOn,
                                                  string fileId,
                                                  string previousVersion,
                                                  string version,
                                                  string filePath,
                                                  List<string> availableInRegions,
                                                  DateTime availableFrom,
                                                  List<string> channels)
        {
            Result<List<Region>> availableInRegionsResult = InScaleFileExtensions.ConvertRegions(availableInRegions);
            if (availableInRegionsResult.IsFailed)
            {
                return Result.Fail<InScaleFile>(availableInRegionsResult.Errors);
            }

            Result<List<Channel>> channelsResult = InScaleFileExtensions.ConvertChannels(channels);
            if (channelsResult.IsFailed)
            {
                return Result.Fail<InScaleFile>(channelsResult.Errors);
            }

            Result<ValueObjects.Version> versionResult = ValueObjects.Version.Create(version);
            if (versionResult.IsFailed)
            {
                return Result.Fail<InScaleFile>(versionResult.Errors);
            }

            Result<ValueObjects.Version> previousVersionResult = ValueObjects.Version.Create(previousVersion);
            if (previousVersionResult.IsFailed)
            {
                return Result.Fail<InScaleFile>(previousVersionResult.Errors);
            }


            InScaleFile file = new InScaleFile(uid,
                                               createdOn,
                                               fileId,
                                               previousVersionResult.Value,
                                               versionResult.Value,
                                               filePath,
                                               availableInRegionsResult.Value,
                                               availableFrom,
                                               channelsResult.Value);
            return Result.Ok(file);
        }

        public bool AvailableInRegion(Region region)
        {
            return AvailableInRegions.Any(x => x.Equals(region));
        }

        public bool ForChannel(Channel channel)
        {
            return Channels.Any(x => x.Equals(channel));
        }

        public void AddFilePath(string filePath)
        {
            FilePath = filePath;
        }
    }
}
