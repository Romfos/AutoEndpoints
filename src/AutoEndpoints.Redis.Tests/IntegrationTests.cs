using AutoEndpoints.Redis.App;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;

namespace AutoEndpoints.Redis.Tests;

[TestClass]
public sealed class IntegrationTests
{
    [TestMethod]
    public async Task RedisTest()
    {
        // arrange
        var webApplicationFactory = new WebApplicationFactory<Program>();
        using var httpClient = webApplicationFactory.CreateClient();

        var expected = new RedisTestModel(1, 2);

        var token = $"{Environment.Version}:{nameof(RedisTest)}";

        // act
        using var response1 = await httpClient.PostAsync(token, JsonContent.Create(expected));
        response1.EnsureSuccessStatusCode();

        using var response2 = await httpClient.GetAsync(token);
        response2.EnsureSuccessStatusCode();
        var actual = await response2.Content.ReadFromJsonAsync<RedisTestModel>();

        // assert
        Assert.AreEqual(expected, actual);
    }
}
