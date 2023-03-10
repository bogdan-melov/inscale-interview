
namespace InScale.Common.Common.Enumeration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class Enumeration<T> where T : struct, IComparable
    {
        public T Id { get; }

        public string Name { get; }

        protected Enumeration()
        {
        }

        protected Enumeration(T id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<TResult> GetAll<TResult>() where TResult : Enumeration<T>, new()
        {
            var type = typeof(TResult);

            var fields = type.GetTypeInfo().GetFields(BindingFlags.Public |
                                                      BindingFlags.Static |
                                                      BindingFlags.DeclaredOnly);
            foreach (var info in fields)
            {
                var instance = new TResult();
                var locatedValue = info.GetValue(instance) as TResult;

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration<T>;
            if (otherValue == null)
            {
                return false;
            }
            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);
            return typeMatches && valueMatches;
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((Enumeration<T>)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
