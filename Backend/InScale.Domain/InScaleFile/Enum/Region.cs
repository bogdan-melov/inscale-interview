namespace InScale.Domain.InScaleFile.Enum
{
    using FluentResults;
    using InScale.Contracts.Exceptions;
    using InScale.Domain.Common;
    using System.Linq;

    public class Region : Enumeration<byte>
    {
        public static readonly Region USA = new Region(0, "USA");
        public static readonly Region Europe = new Region(1, "EUROPE");

        public Region(byte id, string name) : base(id, name)
        {
        }

        public Region()
        {
        }

        public static implicit operator byte(Region region)
        {
            return region.Id;
        }

        public static implicit operator string(Region region)
        {
            return region.Name;
        }

        public static explicit operator Region(byte region)
        {
            return Create(region).Value;
        }

        public static explicit operator Region(string region)
        {
            return Create(region).Value;
        }

        public static Result<Region> Create(string regionName)
        {
            Region region = GetAll<Region>().SingleOrDefault(f => f.Name == regionName);

            if (region == null)
            {
                return Result.Fail<Region>(ResultErrorCodes.ChannelNotValid);
            }

            return Result.Ok(region);
        }
        private static Result<Region> Create(byte region)
        {
            Region accType = GetAll<Region>().SingleOrDefault(f => f.Id == region);

            if (accType == null)
            {
                return Result.Fail<Region>(ResultErrorCodes.ChannelNotValid);
            }

            return Result.Ok(accType);
        }
    }
}
