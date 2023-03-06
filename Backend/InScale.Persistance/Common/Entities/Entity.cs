namespace InScale.Persistance.Common.Entities
{
    using Newtonsoft.Json;
    using System;

    public class Entity<T> where T : class
    {
        [JsonProperty("entityName")]
        public string EntityName { get; private set; } = typeof(T).Name;
       
        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; private set; } 


        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid Uid { get; private set; }

        [JsonConstructor]
        protected Entity(Guid uid, DateTime createdOn)
        {
            Uid = uid;
            CreatedOn = createdOn;
        }
    }
}
