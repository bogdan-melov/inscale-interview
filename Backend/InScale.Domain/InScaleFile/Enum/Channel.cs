namespace InScale.Domain.InScaleFile.Enum
{
    using FluentResults;
    using InScale.Contracts.Exceptions;
    using InScale.Domain.Common;
    using System.Linq;

    public class Channel : Enumeration<byte>
    {
        public static readonly Channel InternalBeta = new Channel(0, "INTERNAL_BETA");
        public static readonly Channel Insiders = new Channel(1, "INSIDERS");
        public static readonly Channel Public = new Channel(2, "PUBLIC");

        public Channel(byte id, string name) : base(id, name)
        {
        }

        public Channel()
        {
        }

        public static implicit operator byte(Channel channel)
        {
            return channel.Id;
        }

        public static implicit operator string(Channel channel)
        {
            return channel.Name;
        }

        public static explicit operator Channel(byte channel)
        {
            return Create(channel).Value;
        }

        public static explicit operator Channel(string channel)
        {
            return Create(channel).Value;
        }

        public static Result<Channel> Create(string channel)
        {
            Channel accType = GetAll<Channel>().SingleOrDefault(f => f.Name == channel);

            if (accType == null)
            {
                return Result.Fail<Channel>(ResultErrorCodes.ChannelNotValid);
            }

            return Result.Ok(accType);
        }
        private static Result<Channel> Create(byte accountType)
        {
            Channel accType = GetAll<Channel>().SingleOrDefault(f => f.Id == accountType);

            if (accType == null)
            {
                return Result.Fail<Channel>(ResultErrorCodes.ChannelNotValid);
            }

            return Result.Ok(accType);
        }
    }
}
