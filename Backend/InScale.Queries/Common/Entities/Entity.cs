namespace InScale.Queries.Common.Entities
{
    using Newtonsoft.Json;
    using System;

    public class Entity
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid Uid { get; private set; }

        [JsonConstructor]
        protected Entity(Guid uid)
        {
            Uid = uid;
        }
    }
}
