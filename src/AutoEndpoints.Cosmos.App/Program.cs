using Microsoft.Azure.Cosmos;

namespace AutoEndpoints.Cosmos.App;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var cosmosClientOptions = new CosmosClientOptions()
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true
        };

        builder.Services.AddCosmosLayouts(
            "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            cosmosClientOptions);

        var app = builder.Build();

        app.MapGetLayout("{partition}/{id}")
            .UseCosmos("db1", "container1", x => x.GetStringRouteValue("partition"), x => x.GetStringRouteValue("id"));

        app.MapPostLayout<CosmosTestModel>("{partition}/{id}")
            .Validate(StatusCodes.Status400BadRequest, (context, model) => model.Value > 3)
            .UseCosmos("db1", "container1", (context, body) => context.GetStringRouteValue("partition"));

        await app.RunAsync();
    }
}