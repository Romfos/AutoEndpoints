using Microsoft.Azure.Cosmos;

namespace AutoEndpoints.Cosmos;

internal sealed class CosmosStorage(string connectionString, CosmosClientOptions? cosmosClientOptions = null)
{
    private readonly CosmosClient cosmosClient = new CosmosClient(connectionString, cosmosClientOptions);

    public ContainerStorage GetContainerStorage(string database, string collection)
    {
        return new ContainerStorage(cosmosClient.GetContainer(database, collection));
    }
}
