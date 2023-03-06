using Microsoft.Azure.Cosmos;

namespace InScale.Persistance.Common.Contracts
{
    public interface ICosmosDbContext
    {
        Container FileContainer { get;  }
    }
}
