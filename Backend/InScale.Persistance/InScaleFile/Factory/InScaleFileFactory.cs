namespace InScale.Persistance.InScaleFile.Factory
{
    using FluentResults;
    using InScale.Domain.InScaleFile.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class InScaleFileFactory
    {
        public static Result<List<InScaleFile>> ToInScaleFiles(this List<Entities.InScaleFile> entities)
        {
            var result = new List<InScaleFile>();

            foreach (var entity in entities)
            {
                Result<InScaleFile> inScaleFileResult = entity.ToInScaleFile();

                if (inScaleFileResult.IsFailed)
                {
                    return Result.Fail(inScaleFileResult.Errors);
                }

                result.Add(inScaleFileResult.Value);
            }

            return Result.Ok(result);
        }

        public static Result<InScaleFile> ToInScaleFile(this Entities.InScaleFile entity) =>
            InScaleFile.Create(entity.Uid,
                               entity.CreatedOn,
                               entity.FileId,
                               entity.PreviousVersion,
                               entity.Version,
                               entity.FilePath,
                               entity.AvailableInRegions,
                               entity.AvailableFrom,
                               entity.Channels);

        public static Entities.InScaleFile ToInScaleFileEntity(this InScaleFile file)
        => new Entities.InScaleFile(file.Uid, 
                                    file.CreatedOn, 
                                    file.FileId, 
                                    file.PreviousVersion.ToString(),
                                    file.Version.ToString(),
                                    file.FilePath,
                                    file.AvailableInRegions.Select(x => x.Name).ToList(),
                                    file.AvailableFrom,
                                    file.Channels.Select(x => x.Name).ToList());
    }
}
