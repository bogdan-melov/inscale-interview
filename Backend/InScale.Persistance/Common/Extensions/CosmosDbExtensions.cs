namespace InScale.Persistance.Common.Extensions
{
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class CosmosDbExtensions
    {
        public static async Task<List<T>> ExecuteQueryAsync<T>(this IQueryable<T> query)
        {
            FeedIterator<T> feedIterator = query.ToFeedIterator();

            var results = new List<T>();

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<T> response = await feedIterator.ReadNextAsync();

                results.AddRange(response);
            }

            return results;
        }
    }
}
