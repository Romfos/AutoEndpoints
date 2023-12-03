using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;

namespace AutoEndpoints.Cosmos;

public static class CosmosLayoutExtensions
{
    public static void AddCosmosLayouts(this IServiceCollection serviceCollection, string connectionString, CosmosClientOptions? cosmosClientOptions = null)
    {
        serviceCollection.AddSingleton(new CosmosStorage(connectionString, cosmosClientOptions));
    }

    public static void UseCosmos(
        this GetRequestBuilder request,
        string database,
        string container,
        Func<HttpContext, string> partitionSelector,
        Func<HttpContext, string> idSelector)
    {
        var cosmosStorage = request.webApplication.Services.GetRequiredService<CosmosStorage>();
        var containerStorage = cosmosStorage.GetContainerStorage(database, container);

        request.webApplication.MapGet(request.pattern, async (HttpContext context) =>
        {
            foreach (var (statusCode, verification) in request.verifications)
            {
                if (verification(context))
                {
                    return Results.StatusCode(statusCode);
                }
            }

            var partition = partitionSelector(context);
            var id = idSelector(context);
            var result = await containerStorage.GetAsync(partition, id);
            return Results.Content(result ?? string.Empty, MediaTypeNames.Application.Json);
        });
    }

    public static void UseCosmos<T>(
        this PostRequestBuilder<T> request,
        string database,
        string container,
        Func<HttpContext, T, string> partitionSelector)
    {
        var cosmosStorage = request.webApplication.Services.GetRequiredService<CosmosStorage>();
        var containerStorage = cosmosStorage.GetContainerStorage(database, container);

        request.webApplication.MapPost(request.pattern, async (HttpContext context, [FromBody] T value) =>
        {
            foreach (var (statusCode, verification) in request.verifications)
            {
                if (verification(context))
                {
                    return Results.StatusCode(statusCode);
                }
            }

            foreach (var (statusCode, validation) in request.validations)
            {
                if (validation(context, value))
                {
                    return Results.StatusCode(statusCode);
                }
            }

            var partition = partitionSelector(context, value);
            await containerStorage.UpsertAsync(partition, value);
            return Results.Ok();
        });
    }
}
