using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.Redis;

public sealed class RedisPostEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, string>? keySelector;

    public RedisPostEndpointBuilder<T> Key(Func<HttpContext, string> selector)
    {
        keySelector = selector;
        return this;
    }

    public RedisPostEndpointBuilder<T> KeyFromRoute(string routeParameterName)
    {
        keySelector = context => context.GetRouteValue(routeParameterName)?.ToString()!;
        return this;
    }

    public RouteHandlerBuilder Build()
    {
        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(Key));
        }

        var redisStorage = webApplication.Services.GetRequiredService<RedisDataProvider>();

        return webApplication.MapPost(pattern, async (HttpContext context, T value) =>
        {
            var key = keySelector(context);
            await redisStorage.SetAsync<T>(key, value);
            return Results.Ok();
        });
    }
}