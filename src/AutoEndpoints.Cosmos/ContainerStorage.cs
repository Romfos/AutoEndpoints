using Microsoft.Azure.Cosmos;
using System.Net;

namespace AutoEndpoints.Cosmos;

internal sealed class ContainerStorage(Container container)
{
    private static readonly ItemRequestOptions itemRequestOptions = new ItemRequestOptions() { EnableContentResponseOnWrite = true };

    public async Task<string?> GetAsync(string partition, string id)
    {
        try
        {
            return await container.ReadItemAsync<string>(id, new PartitionKey(partition));
        }
        catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpsertAsync<T>(string partition, T value)
    {
        await container.UpsertItemAsync(value, new PartitionKey(partition), itemRequestOptions);
    }
}
