using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.Redis;

public static class RedisAutoEndpointsExtensions
{
    public static void AddRedisEndpoints(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddSingleton(new RedisDataProvider(connectionString));
    }

    public static RedisGetEndpointBuilder<T> MapRedisGetEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new RedisGetEndpointBuilder<T>(webApplication, pattern);
    }

    public static RedisPostEndpointBuilder<T> MapRedisPostEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new RedisPostEndpointBuilder<T>(webApplication, pattern);
    }
}
