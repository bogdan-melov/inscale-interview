namespace InScale.Domain.InScaleFile.ValueObjects
{
    using FluentResults;
    using InScale.Domain.Common.ValueObject;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Version : ValueObject
    {
        public int Major { get; }

        public int Minor { get; }

        public int Micro { get; }

        public static Version Empty = new Version(0, 0, 0);

        public Version(int major, int minor, int micro)
        {
            Major = major;
            Minor = minor;
            Micro = micro;
        }

        public static Result<Version> Create(int major, int minor, int micro)
        {
            return new Version(major, minor, micro);
        }

        public static Result<Version> Create(string version)
        {
            try
            {
                if (version == null)
                {
                    return Result.Ok(Empty);
                }

                List<string> versionParts = version.Split('.').ToList();

                return Result.Ok(new Version(Convert.ToInt32(versionParts[0]),
                                             Convert.ToInt32(versionParts[1]),
                                             Convert.ToInt32(versionParts[2])));
            }
            catch (IndexOutOfRangeException ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Major;
            yield return Minor;
            yield return Micro;
        }

        public override string ToString() { return $"{Major}.{Minor}.{Micro}"; }
    }
}
