using Microsoft.Azure.Cosmos;

namespace AutoEndpoints.Cosmos;

internal sealed class CosmosDataProvider(string connectionString, CosmosClientOptions? cosmosClientOptions = null)
{
    private readonly CosmosClient cosmosClient = new CosmosClient(connectionString, cosmosClientOptions);

    public CosmosContainerDataProvider GetContainerStorage(string database, string collection)
    {
        return new CosmosContainerDataProvider(cosmosClient.GetContainer(database, collection));
    }
}
