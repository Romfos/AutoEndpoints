using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AutoEndpoints.Dapper;

public sealed class DapperGetEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, IDbConnection, Task<T>>? query;

    public DapperGetEndpointBuilder<T> Query(Func<HttpContext, IDbConnection, Task<T>> query)
    {
        if (this.query != null)
        {
            throw new ArgumentException($"Query has already been configured");
        }

        this.query = query;
        return this;
    }

    public DapperGetEndpointBuilder<T> Query(Func<IDbConnection, Task<T>> query)
    {
        if (this.query != null)
        {
            throw new ArgumentException($"Query has already been configured");
        }

        this.query = (context, connection) => query(connection);
        return this;
    }

    public RouteHandlerBuilder Build()
    {
        if (query == null)
        {
            throw new ArgumentException("Query is required");
        }

        var sqlConnection = webApplication.Services.GetRequiredService<IDbConnection>();

        return webApplication.MapGet(pattern, async (HttpContext context) =>
        {
            var result = await query(context, sqlConnection);
            return Results.Ok(result);
        });
    }
}
