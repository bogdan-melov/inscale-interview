namespace InScale.Queries.Common.Services
{
    using InScale.Queries.Common.Contracts;
    using Microsoft.Azure.Cosmos;

    public class InScaleFileReadOnlyDbContext : ReadOnlyDbContext, IInScaleFileReadOnlyDbContext
    {
        public InScaleFileReadOnlyDbContext(Container _inScaleFileContainer): base(_inScaleFileContainer)
        {
        }
    }
}
