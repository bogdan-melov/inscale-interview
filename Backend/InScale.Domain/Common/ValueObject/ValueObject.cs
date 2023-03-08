namespace InScale.Domain.Common.ValueObject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            var valueObject = obj as ValueObject;

            if (ReferenceEquals(valueObject, null))
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + (obj?.GetHashCode() ?? 0);
                    }
                });
        }
    }
}