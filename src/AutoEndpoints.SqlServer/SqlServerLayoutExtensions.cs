using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoEndpoints.SqlServer;

public static class SqlServerLayoutExtensions
{
    public static void AddSqlServerLayouts<TContext>(this IServiceCollection serviceCollection, string connectionString)
        where TContext : DbContext
    {
        serviceCollection.AddSqlServer<TContext>(connectionString);
    }

    public static void UseSqlServer<TContext>(this GetRequestBuilder request, Func<HttpContext, TContext, Task<object?>> query)
        where TContext : DbContext
    {
        request.webApplication.MapGet(request.pattern, async (HttpContext context, TContext dbContext) =>
        {
            foreach (var (statusCode, verification) in request.verifications)
            {
                if (verification(context))
                {
                    return Results.StatusCode(statusCode);
                }
            }
            var model = await query(context, dbContext);
            return Results.Ok(model);
        });
    }

    public static void UseSqlServer<TContext, TModel>(this PostRequestBuilder<TModel> request, Action<HttpContext, TModel, TContext> mutator)
        where TContext : DbContext
    {
        request.webApplication.MapPost(request.pattern, async (HttpContext context, TContext dbContext, [FromBody] TModel value) =>
        {
            foreach (var (statusCode, verification) in request.verifications)
            {
                if (verification(context))
                {
                    return Results.StatusCode(statusCode);
                }
            }

            foreach (var (statusCode, validation) in request.validations)
            {
                if (validation(context, value))
                {
                    return Results.StatusCode(statusCode);
                }
            }

            mutator(context, value, dbContext);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
