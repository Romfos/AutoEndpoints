using Microsoft.AspNetCore.Builder;

namespace AutoEndpoints.Dapper;

public static class DapperSqlServerAutoEndpointsExtensions
{
    public static DapperGetEndpointBuilder<T> MapDapperGetEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new DapperGetEndpointBuilder<T>(webApplication, pattern);
    }

    public static DapperPostEndpointBuilder<T> MapDapperPostEndpoint<T>(this WebApplication webApplication, string pattern)
    {
        return new DapperPostEndpointBuilder<T>(webApplication, pattern);
    }
}
