namespace InScale.Domain.Common
{
    using System;

    public class Entity
    {
        public Guid Uid { get; }

        public Entity(Guid uiId)
        {
            Uid = uiId;
        }
    }
}
