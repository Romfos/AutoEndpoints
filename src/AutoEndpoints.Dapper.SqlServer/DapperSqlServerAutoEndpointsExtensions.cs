using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AutoEndpoints.Dapper.SqlServer;

public static class DapperSqlServerAutoEndpointsExtensions
{
    public static void AddDapperSqlServerEndpoints(this IServiceCollection serviceCollection, string connectionString)
    {
        var sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();
        serviceCollection.AddSingleton<IDbConnection>(sqlConnection);
    }

    public static DapperGetEndpointBuilder<T> MapDapperGetEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new DapperGetEndpointBuilder<T>(webApplication, pattern);
    }

    public static DapperPostEndpointBuilder<T> MapDapperPostEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new DapperPostEndpointBuilder<T>(webApplication, pattern);
    }
}
