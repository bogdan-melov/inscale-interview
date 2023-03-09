namespace InScale.Common.InScaleFile.Enum
{
    using FluentResults;
    using InScale.Common.Common.Enumeration;
    using InScale.Common.Common.Result;
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
            Channel _channel = GetAll<Channel>().SingleOrDefault(f => f.Name == channel);

            if (_channel == null)
            {
                return Result.Fail<Channel>(ResultErrorCodes.ChannelNotValid);
            }

            return Result.Ok(_channel);
        }
        private static Result<Channel> Create(byte channel)
        {
            Channel _channel = GetAll<Channel>().SingleOrDefault(f => f.Id == channel);

            if (_channel == null)
            {
                return Result.Fail<Channel>(ResultErrorCodes.ChannelNotValid);
            }

            return Result.Ok(_channel);
        }
    }
}
