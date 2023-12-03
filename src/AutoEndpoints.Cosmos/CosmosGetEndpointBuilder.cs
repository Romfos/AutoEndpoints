using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.Cosmos;

public sealed class CosmosGetEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, string>? databaseSelector;
    private Func<HttpContext, string>? collectionSelector;
    private Func<HttpContext, string>? partitionSelector;
    private Func<HttpContext, string>? idSelector;

    public CosmosGetEndpointBuilder<T> Database(string database)
    {
        if (databaseSelector != null)
        {
            throw new ArgumentException($"Database has already been configured");
        }

        databaseSelector = context => database;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Database(Func<HttpContext, string> selector)
    {
        if (databaseSelector != null)
        {
            throw new ArgumentException($"Database has already been configured");
        }

        databaseSelector = selector;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Collection(string collection)
    {
        if (collectionSelector != null)
        {
            throw new ArgumentException($"Collection has already been configured");
        }

        collectionSelector = context => collection;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Collection(Func<HttpContext, string> selector)
    {
        if (collectionSelector != null)
        {
            throw new ArgumentException($"Collection has already been configured");
        }

        collectionSelector = selector;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Partition(string partition)
    {
        if (partitionSelector != null)
        {
            throw new ArgumentException($"Partition has already been configured");
        }

        partitionSelector = context => partition;
        return this;
    }

    public CosmosGetEndpointBuilder<T> PartitionFromRoute(string routeParameterName)
    {
        if (partitionSelector != null)
        {
            throw new ArgumentException($"Partition has already been configured");
        }

        partitionSelector = context => context.GetRouteValue(routeParameterName)?.ToString()!;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Partition(Func<HttpContext, string> selector)
    {
        if (partitionSelector != null)
        {
            throw new ArgumentException($"Partition has already been configured");
        }

        partitionSelector = selector;
        return this;
    }

    public CosmosGetEndpointBuilder<T> IdFromRoute(string routeParameterName)
    {
        if (idSelector != null)
        {
            throw new ArgumentException($"Id has already been configured");
        }

        idSelector = context => context.GetRouteValue(routeParameterName)?.ToString()!;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Id(string id)
    {
        if (idSelector != null)
        {
            throw new ArgumentException($"Id has already been configured");
        }

        idSelector = context => id;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Id(Func<HttpContext, string> selector)
    {
        if (idSelector != null)
        {
            throw new ArgumentException($"Id has already been configured");
        }

        idSelector = selector;
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
        if (idSelector == null)
        {
            throw new ArgumentException("Id is required");
        }

        var cosmosDataProvider = webApplication.Services.GetRequiredService<CosmosDataProvider>();

        return webApplication.MapGet(pattern, async (HttpContext context) =>
        {
            var database = databaseSelector(context);
            var collection = collectionSelector(context);
            var partition = partitionSelector(context);
            var id = idSelector(context);

            var container = cosmosDataProvider.GetContainerStorage(database, collection);
            var result = await container.GetAsync<T>(partition, id);
            return Results.Ok(result);
        });
    }
}
