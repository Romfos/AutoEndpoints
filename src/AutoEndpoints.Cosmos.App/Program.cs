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

        builder.Services.AddCosmosEndpoints(
            "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            cosmosClientOptions);

        var app = builder.Build();

        app.MapCosmosGetEndpoint<CosmosTestModel>("{partition}/{id}")
            .Database("db1")
            .Collection("container1")
            .PartitionFromRoute("partition")
            .IdFromRoute("id")
            .Build();

        app.MapCosmosPostEndpoint<CosmosTestModel>("{partition}/{id}")
            .Database("db1")
            .Collection("container1")
            .PartitionFromRoute("partition")
            .Build();

        await app.RunAsync();
    }
}