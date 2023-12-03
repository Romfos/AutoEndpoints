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

    public CosmosGetEndpointBuilder<T> Database(Func<HttpContext, string> selector)
    {
        databaseSelector = selector;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Collection(Func<HttpContext, string> selector)
    {
        collectionSelector = selector;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Partition(Func<HttpContext, string> selector)
    {
        partitionSelector = selector;
        return this;
    }

    public CosmosGetEndpointBuilder<T> Id(Func<HttpContext, string> selector)
    {
        idSelector = selector;
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
        if (idSelector == null)
        {
            throw new ArgumentNullException(nameof(Id));
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
