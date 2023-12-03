using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AutoEndpoints.Dapper.SqlServer;

public sealed class DapperGetEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, IDbConnection, Task<T>>? query;

    public DapperGetEndpointBuilder<T> Query(Func<HttpContext, IDbConnection, Task<T>> query)
    {
        this.query = query;
        return this;
    }

    public RouteHandlerBuilder Build()
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(Query));
        }

        var sqlConnection = webApplication.Services.GetRequiredService<IDbConnection>();

        return webApplication.MapGet(pattern, async (HttpContext context) =>
        {
            var result = await query(context, sqlConnection);
            return Results.Ok(result);
        });
    }
}
