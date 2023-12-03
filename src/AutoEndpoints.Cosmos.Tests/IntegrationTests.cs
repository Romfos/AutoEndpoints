using AutoEndpoints.Cosmos.App;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;

namespace AutoEndpoints.Cosmos.Tests;

[TestClass]
public sealed class IntegrationTests
{
    [TestMethod]
    public async Task CosmosTest()
    {
        // arrange
        var cosmosClientOptions = new CosmosClientOptions()
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true
        };

        var cosmosClient = new CosmosClient(
            "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            cosmosClientOptions);

        await cosmosClient.CreateDatabaseIfNotExistsAsync("db1", 4000);
        var database = cosmosClient.GetDatabase("db1");
        await database.CreateContainerIfNotExistsAsync("container1", "/partition");

        var webApplicationFactory = new WebApplicationFactory<Program>();
        using var httpClient = webApplicationFactory.CreateClient();

        var expected = new CosmosTestModel(Environment.Version.ToString(), nameof(CosmosTest), 2);

        var path = $"{expected.Partition}/{expected.Id}";

        // act
        using var response1 = await httpClient.PostAsync(path, JsonContent.Create(expected));
        response1.EnsureSuccessStatusCode();

        using var response2 = await httpClient.GetAsync(path);
        response2.EnsureSuccessStatusCode();
        var actual = await response2.Content.ReadFromJsonAsync<CosmosTestModel>();

        // assert
        Assert.AreEqual(expected, actual);
    }
}
