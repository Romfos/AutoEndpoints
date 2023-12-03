using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AutoEndpoints.Dapper;

public sealed class DapperPostEndpointBuilder<T>(WebApplication webApplication, string pattern)
{
    private Func<HttpContext, IDbConnection, T, Task>? command;

    public DapperPostEndpointBuilder<T> Command(Func<HttpContext, IDbConnection, T, Task> command)
    {
        if (this.command != null)
        {
            throw new ArgumentException($"Command has already been configured");
        }

        this.command = command;
        return this;
    }

    public DapperPostEndpointBuilder<T> Command(Func<IDbConnection, T, Task> command)
    {
        if (this.command != null)
        {
            throw new ArgumentException($"Command has already been configured");
        }

        this.command = (context, connection, value) => command(connection, value);
        return this;
    }

    public RouteHandlerBuilder Build()
    {
        if (command == null)
        {
            throw new ArgumentException("Command is required");
        }

        var sqlConnection = webApplication.Services.GetRequiredService<IDbConnection>();

        return webApplication.MapPost(pattern, async (HttpContext context, T value) =>
        {
            await command(context, sqlConnection, value);
            return Results.Ok();
        });
    }
}