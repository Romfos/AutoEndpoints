using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.Redis;

public sealed class RedisGetEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, string>? keySelector;

    public RedisGetEndpointBuilder<T> Key(Func<HttpContext, string> selector)
    {
        if (keySelector != null)
        {
            throw new ArgumentException($"Key has already been configured");
        }

        keySelector = selector;
        return this;
    }

    public RedisGetEndpointBuilder<T> KeyFromRoute(string routeParameterName)
    {
        if (keySelector != null)
        {
            throw new ArgumentException($"Key has already been configured");
        }

        keySelector = context => context.GetRouteValue(routeParameterName)?.ToString()!;
        return this;
    }

    public RouteHandlerBuilder Build()
    {
        if (keySelector == null)
        {
            throw new ArgumentException("Key is required");
        }

        var redisStorage = webApplication.Services.GetRequiredService<RedisDataProvider>();

        return webApplication.MapGet(pattern, async (HttpContext context) =>
        {
            var key = keySelector(context);
            var result = await redisStorage.GetAsync<T>(key);
            return Results.Ok(result);
        });
    }
}
