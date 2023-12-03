using Dapper;

namespace AutoEndpoints.Dapper.SqlServer.App;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDapperSqlServerEndpoints("Data Source=localhost;Initial Catalog=Database1;User Id=sa;Password=a123456789!;TrustServerCertificate=True");

        var app = builder.Build();

        app.MapDapperGetEndpoint<SqlServerTestModel?>("/{id}")
            .Query((context, connection) => connection.QueryFirstOrDefaultAsync<SqlServerTestModel>(
                "SELECT * FROM SqlServerTestModels WHERE id = @Id",
                new
                {
                    Id = context.Request.RouteValues["id"]!.ToString()!
                }))
            .Build();

        app.MapDapperPostEndpoint<SqlServerTestModel>("/")
            .Command((connection, value) => connection.ExecuteAsync(
                "INSERT INTO SqlServerTestModels (ID, Value) VALUES (@ID, @Value)",
                value))
            .Build();


        await app.RunAsync();
    }
}
