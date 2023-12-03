using Microsoft.Azure.Cosmos;
using System.Net;

namespace AutoEndpoints.Cosmos;

internal sealed class CosmosContainerDataProvider(Container container)
{
    private static readonly ItemRequestOptions itemRequestOptions = new() { EnableContentResponseOnWrite = true };

    public async Task<T?> GetAsync<T>(string partition, string id)
    {
        try
        {
            return await container.ReadItemAsync<T>(id, new PartitionKey(partition));
        }
        catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async Task UpsertAsync<T>(string partition, T value)
    {
        await container.UpsertItemAsync(value, new PartitionKey(partition), itemRequestOptions);
    }
}
