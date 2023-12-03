using AutoEndpoints.SqlServer.App;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;

namespace AutoEndpoints.SqlServer.Tests;

[TestClass]
public sealed class IntegrationTests
{
    [TestMethod]
    public async Task SqlServerTest()
    {
        // arrange

        var options = new DbContextOptionsBuilder<SqlServerContext>()
            .UseSqlServer("Data Source=localhost;Initial Catalog=Database1;User Id=sa;Password=a123456789!")
            .Options;

        var id = $"{Environment.Version}:{nameof(SqlServerTest)}";

        using (var context = new SqlServerContext(options))
        {
            context.Database.EnsureCreated();

            context.Database.ExecuteSqlRaw(
                """
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'SqlServerTestModels')
            BEGIN
                CREATE TABLE [dbo].[SqlServerTestModels]
                (
            	    [Id] NVARCHAR(256) NOT NULL PRIMARY KEY,
            	    [Value] INT NOT NULL
                )
            END 
            """);

            var current = await context.SqlServerTestModels.SingleOrDefaultAsync(x => x.Id == id);
            if (current != null)
            {
                context.SqlServerTestModels.Remove(current);
                await context.SaveChangesAsync();
            }
        }

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
