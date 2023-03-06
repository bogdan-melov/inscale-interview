namespace InScale.Domain.InScaleFile
{
    using FluentResults;
    using InScale.Domain.Common;
    using InScale.Domain.InScaleFile.Enum;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InScaleFile : Entity
    {
        public string FileId;

        public string PreviousVersion;

        public string Version;

        public string Url;

        public List<Region> AvailableInRegions;

        public DateTime AvailableFrom;

        public List<Channel> Channels;

        public InScaleFile(Guid uid,
                           DateTime createdOn,
                           string fileId,
                           string previousVersion,
                           string version,
                           string url,
                           List<Region> availableInRegions,
                           DateTime availableFrom,
                           List<Channel> channels) : base(uid, createdOn)
        {
            FileId = fileId;
            PreviousVersion = previousVersion;
            Version = version;
            Url = url;
            AvailableInRegions = availableInRegions;
            AvailableFrom = availableFrom;
            Channels = channels;
        }

        public Result<InScaleFile> CreateFromDB(Guid uid,
                                                DateTime createdOn,
                                                string fileId,
                                                string previousVersion,
                                                string version,
                                                string url,
                                                List<string> availableInRegions,
                                                DateTime availableFrom,
                                                List<string> channels)
        {
            List<Region> availableInRegionsValues = new List<Region>();

            List<Result<Region>> availableInRegionsEnums = availableInRegions.Select(x => Region.Create(x)).ToList();

            foreach (Result<Region> regionResult in availableInRegionsEnums)
            {
                if (regionResult.IsFailed)
                {
                    return Result.Fail<InScaleFile>(regionResult.Errors);
                }

                availableInRegionsValues.Add(regionResult.Value);
            }

            List<Channel> channelsValues = new List<Channel>();

            List<Result<Channel>> channelsResults = channels.Select(x => Channel.Create(x)).ToList();
            
            foreach(Result<Channel> channelResult in channelsResults)
            {
                if (channelResult.IsFailed)
                {
                    return Result.Fail<InScaleFile>(channelResult.Errors);
                }

                channelsValues.Add(channelResult.Value);
            }

            InScaleFile file = new InScaleFile(uid,
                                               createdOn,
                                               fileId,
                                               previousVersion,
                                               version,
                                               url,
                                               availableInRegionsValues,
                                               availableFrom,
                                               channelsValues);

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
    }
}
