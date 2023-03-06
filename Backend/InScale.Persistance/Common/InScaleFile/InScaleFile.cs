namespace InScale.Persistance.Common.InScaleFile
{
    using InScale.Persistance.Common.Entities;
    using Microsoft.Azure.Cosmos;
    using System;

    public class InScaleFile : Entity<InScaleFile>
    {
        protected InScaleFile(Guid uid, DateTime createdOn) : base(uid, createdOn)
        {
        }
    }
}
