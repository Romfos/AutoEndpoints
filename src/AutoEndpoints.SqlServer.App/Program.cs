using Microsoft.AspNetCore.Routing.Patterns;

namespace AutoEndpoints.SqlServer.App;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        var routePattern = RoutePatternFactory.Parse("/{id}");
        var requestDelegateResult = RequestDelegateFactory.Create(() =>
        {

        }, new RequestDelegateFactoryOptions()
        {
            ServiceProvider = app.Services,
            RouteParameterNames = routePattern.Parameters.Select(x => x.Name)
        });


        app.MapGet("/{id}", async (httpContext) =>
        {
            await requestDelegateResult.RequestDelegate(httpContext);
        });


        await app.RunAsync();
    }
}
