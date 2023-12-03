using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.Cosmos;

public static class CosmosAutoEndpointsExtensions
{
    public static void AddCosmosEndpoints(this IServiceCollection serviceCollection, string connectionString, CosmosClientOptions? cosmosClientOptions = null)
    {
        serviceCollection.AddSingleton(new CosmosDataProvider(connectionString, cosmosClientOptions));
    }

    public static CosmosGetEndpointBuilder<T> MapCosmosGetEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new CosmosGetEndpointBuilder<T>(webApplication, pattern);
    }

    public static CosmosPostEndpointBuilder<T> MapCosmosPostEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new CosmosPostEndpointBuilder<T>(webApplication, pattern);
    }
}
