namespace AutoEndpoints.Redis.App;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRedisEndpoints("localhost:6379");

        var app = builder.Build();

        app.MapRedisGetEndpoint<RedisTestModel>("{id}")
            .Key(context => context.Request.RouteValues["id"]!.ToString()!)
            .Build();

        app.MapRedisPostEndpoint<RedisTestModel>("{id}")
            .Key(context => context.Request.RouteValues["id"]!.ToString()!)
            .Build();

        await app.RunAsync();
    }
}