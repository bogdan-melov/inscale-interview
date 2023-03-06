
namespace InScale.Domain.Common
{
    using System;

    public class Entity
    {
        public Guid Uid { get; }

        public DateTime CreatedOn { get; }

        public Entity(Guid uiId, DateTime createdOn)
        {
            Uid = uiId;
            CreatedOn = createdOn;
        }
    }
}
