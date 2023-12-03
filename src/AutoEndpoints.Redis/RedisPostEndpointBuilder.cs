using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.Redis;

public sealed class RedisPostEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, string>? keySelector;

    public RedisPostEndpointBuilder<T> Key(Func<HttpContext, string> keySelector)
    {
        this.keySelector = keySelector;
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