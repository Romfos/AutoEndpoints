namespace AutoEndpoints.Redis.App;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRedisLayouts("localhost:6379");

        var app = builder.Build();

        app.MapGetLayout("{id}")
            .UseRedis(context => context.GetStringRouteValue("id"));

        app.MapPostLayout<RedisTestModel>("{id}")
            .Validate(StatusCodes.Status400BadRequest, (context, model) => model.Y > 3)
            .UseRedis((context, body) => context.GetStringRouteValue("id"));

        await app.RunAsync();
    }
}