
namespace InScale.Contracts.InScaleFile.Repositories
{
    using FluentResults;
    using InScale.Domain.InScaleFile.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IInScaleFileRepository
    {
        Task<Result<List<InScaleFile>>> GetInScaleFilesByFileId(string fileId);

        Task<Result> SaveInScaleFile(InScaleFile file);
    }
}
