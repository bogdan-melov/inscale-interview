namespace InScale.Queries.InScaleFile.Factory
{
    using FluentResults;
    using InScale.Domain.InScaleFile.Entities;

    public static class InScaleFileFactory
    {
        public static Result<InScaleFile> ToInScaleFile(this Entities.InScaleFile entity) =>
            InScaleFile.Create(entity.Uid,
                               entity.FileId,
                               entity.PreviousVersion,
                               entity.Version,
                               entity.FilePath,
                               entity.AvailableInRegions,
                               entity.AvailableFrom,
                               entity.Channels);
    }
}
