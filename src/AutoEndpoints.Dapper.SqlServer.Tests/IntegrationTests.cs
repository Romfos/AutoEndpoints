using AutoEndpoints.Dapper.SqlServer.App;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;

namespace AutoEndpoints.Dapper.SqlServer.Tests;

[TestClass]
public sealed class IntegrationTests
{
    [TestMethod]
    public async Task DapperSqlServerTest()
    {
        // arrange

        var id = $"{Environment.Version}:{nameof(DapperSqlServerTest)}";

        var sqlConnection = new SqlConnection("Data Source=localhost;Initial Catalog=Database1;User Id=sa;Password=a123456789!;TrustServerCertificate=True");
        await sqlConnection.OpenAsync();

        await sqlConnection.QueryAsync("""
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Database1')
            BEGIN
              CREATE DATABASE Database1;
            END;
            """);

        await sqlConnection.QueryAsync("""
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'SqlServerTestModels')
            BEGIN
                CREATE TABLE [dbo].[SqlServerTestModels]
                (
            	    [Id] NVARCHAR(256) NOT NULL PRIMARY KEY,
            	    [Value] INT NOT NULL
                )
            END 
            """);

        await sqlConnection.QueryAsync("""
            DELETE FROM [dbo].[SqlServerTestModels] WHERE Id = @Id;
            """, new { Id = id });

        var webApplicationFactory = new WebApplicationFactory<Program>();
        using var httpClient = webApplicationFactory.CreateClient();

        var expected = new SqlServerTestModel(id, 2);

        // act
        using var response1 = await httpClient.PostAsync("/", JsonContent.Create(expected));
        response1.EnsureSuccessStatusCode();

        using var response2 = await httpClient.GetAsync($"/{expected.Id}");
        response2.EnsureSuccessStatusCode();
        var actual = await response2.Content.ReadFromJsonAsync<SqlServerTestModel>();

        // assert
        Assert.AreEqual(expected, actual);
    }
}
