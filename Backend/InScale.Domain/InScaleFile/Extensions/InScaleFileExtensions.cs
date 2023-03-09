namespace InScale.Domain.InScaleFile.Extensions
{
    using FluentResults;
    using InScale.Common.InScaleFile.Enum;
    using InScaleFile.Entities;
    using System.Collections.Generic;
    using System.Linq;
    public static class InScaleFileExtensions
    {
        public static List<InScaleFile> SortByVersion(this List<InScaleFile> inScaleFiles)
            => inScaleFiles.OrderBy(x => x.Version.Major)
                           .ThenBy(x => x.Version.Minor)
                           .ThenBy(x => x.Version.Micro).ToList();

        public static Result<List<Region>> ConvertRegions(List<string> regions)
        {
            List<Region> regionValues = new List<Region>();

            List<Result<Region>> availableInRegionsEnums = regions.Select(x => Region.Create(x)).ToList();

            foreach (Result<Region> regionResult in availableInRegionsEnums)
            {
                if (regionResult.IsFailed)
                {
                    return Result.Fail<List<Region>>(regionResult.Errors);
                }

                regionValues.Add(regionResult.Value);
            }

            return Result.Ok(regionValues);
        }

        public static Result<List<Channel>> ConvertChannels(List<string> channels)
        {
            List<Channel> channelsValues = new List<Channel>();

            List<Result<Channel>> channelsResults = channels.Select(x => Channel.Create(x)).ToList();

            foreach (Result<Channel> channelResult in channelsResults)
            {
                if (channelResult.IsFailed)
                {
                    return Result.Fail<List<Channel>>(channelResult.Errors);
                }

                channelsValues.Add(channelResult.Value);
            }

            return Result.Ok(channelsValues);
        }

        public static bool IsUpperVersionOf(this ValueObjects.Version leftVersion, ValueObjects.Version rightVersion)
        {
            return (leftVersion.Major > rightVersion.Major) ||
                   (leftVersion.Major >= rightVersion.Major && leftVersion.Minor > rightVersion.Minor) ||
                   (leftVersion.Major >= rightVersion.Major && leftVersion.Minor >= rightVersion.Minor && leftVersion.Micro >= rightVersion.Micro);
        }
    }
}
