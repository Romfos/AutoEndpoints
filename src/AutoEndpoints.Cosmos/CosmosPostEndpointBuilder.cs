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
        if (databaseSelector != null)
        {
            throw new ArgumentException($"Database has already been configured");
        }

        databaseSelector = context => database;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Database(Func<HttpContext, string> selector)
    {
        if (databaseSelector != null)
        {
            throw new ArgumentException($"Database has already been configured");
        }

        databaseSelector = selector;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Collection(string collection)
    {
        if (collectionSelector != null)
        {
            throw new ArgumentException($"Collection has already been configured");
        }

        collectionSelector = context => collection;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Collection(Func<HttpContext, string> selector)
    {
        if (collectionSelector != null)
        {
            throw new ArgumentException($"Collection has already been configured");
        }

        collectionSelector = selector;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Partition(string partition)
    {
        if (partitionSelector != null)
        {
            throw new ArgumentException($"Partition has already been configured");
        }

        partitionSelector = context => partition;
        return this;
    }

    public CosmosPostEndpointBuilder<T> PartitionFromRoute(string routeParameterName)
    {
        if (partitionSelector != null)
        {
            throw new ArgumentException($"Partition has already been configured");
        }

        partitionSelector = context => context.GetRouteValue(routeParameterName)?.ToString()!;
        return this;
    }

    public CosmosPostEndpointBuilder<T> Partition(Func<HttpContext, string> selector)
    {
        if (partitionSelector != null)
        {
            throw new ArgumentException($"Partition has already been configured");
        }

        partitionSelector = selector;
        return this;
    }

    public RouteHandlerBuilder Build()
    {
        if (databaseSelector == null)
        {
            throw new ArgumentException("Database is required");
        }
        if (collectionSelector == null)
        {
            throw new ArgumentException("Collection is required");
        }
        if (partitionSelector == null)
        {
            throw new ArgumentException("Partition is required");
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