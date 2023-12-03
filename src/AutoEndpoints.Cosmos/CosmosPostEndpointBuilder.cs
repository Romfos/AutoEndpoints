using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.Cosmos;

public sealed class CosmosPostEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, string>? databaseSelector;
    private Func<HttpContext, string>? collectionSelector;
    private Func<HttpContext, string>? partitionSelector;

    public CosmosPostEndpointBuilder<T> Database(string database)
    {
        databaseSelector = context => database;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Database(Func<HttpContext, string> selector)
    {
        databaseSelector = selector;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Collection(string collection)
    {
        collectionSelector = context => collection;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Collection(Func<HttpContext, string> selector)
    {
        collectionSelector = selector;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Partition(string partition)
    {
        partitionSelector = context => partition;
        return this;
    }

    public CosmosPostEndpointBuilder<T> PartitionFromRoute(string routeParameterName)
    {
        partitionSelector = context => context.GetRouteValue(routeParameterName)?.ToString()!;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Partition(Func<HttpContext, string> selector)
    {
        partitionSelector = selector;
        return this;
    }

    public RouteHandlerBuilder Build()
    {
        if (databaseSelector == null)
        {
            throw new ArgumentNullException(nameof(Database));
        }
        if (collectionSelector == null)
        {
            throw new ArgumentNullException(nameof(Collection));
        }
        if (partitionSelector == null)
        {
            throw new ArgumentNullException(nameof(Partition));
        }

        var cosmosDataProvider = webApplication.Services.GetRequiredService<CosmosDataProvider>();

        return webApplication.MapPost(pattern, async (HttpContext context, T value) =>
        {
            var database = databaseSelector(context);
            var collection = collectionSelector(context);
            var partition = partitionSelector(context);

            var container = cosmosDataProvider.GetContainerStorage(database, collection);
            await container.UpsertAsync(partition, value);
            return Results.Ok();
        });
    }
}