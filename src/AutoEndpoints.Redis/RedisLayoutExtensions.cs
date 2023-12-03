using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;

namespace AutoEndpoints.Redis;

public static class RedisLayoutExtensions
{
    public static void AddRedisLayouts(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddSingleton(new RedisStorage(connectionString));
    }

    public static void UseRedis(this GetRequestBuilder request, Func<HttpContext, string> keySelector)
    {
        var redisStorage = request.webApplication.Services.GetRequiredService<RedisStorage>();

        request.webApplication.MapGet(request.pattern, async (HttpContext context) =>
        {
            foreach (var (statusCode, verification) in request.verifications)
            {
                if (verification(context))
                {
                    return Results.StatusCode(statusCode);
                }
            }

            var key = keySelector(context);
            var result = await redisStorage.GetAsync(key);
            return Results.Content(result ?? string.Empty, MediaTypeNames.Application.Json);
        });
    }

    public static void UseRedis<T>(this PostRequestBuilder<T> request, Func<HttpContext, T, string> keySelector)
    {
        var redisStorage = request.webApplication.Services.GetRequiredService<RedisStorage>();

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

            var key = keySelector(context, value);
            await redisStorage.SetAsync(key, value);
            return Results.Ok();
        });
    }
}
