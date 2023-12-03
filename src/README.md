# Overview

AutoEndpoints is ASP.NET Core based framework for rapid WebAPI development

Main features:
1) Automatic creating endpoints for databases.
2) Access verification. Allow to control access to specific endpoints.
3) Validation support. Allow to validate post body requests.

Supported databases:
- [Redis](https://www.nuget.org/packages/AutoEndpoints.Redis) 
- [Microsoft Azure Cosmos DB](https://www.nuget.org/packages/AutoEndpoints.Cosmos)
- [Microsoft SQL Server](https://www.nuget.org/packages/AutoEndpoints.SqlServer)

Supported platforms:
 - .NET 6
 - .NET 7 
 - .NET 8

# Example

```csharp
var builder = WebApplication.CreateBuilder(args);

// register redis layouts
builder.Services.AddRedisLayouts("localhost:6379");

var app = builder.Build();

// map get endpoint
app.MapGetLayout<RedisTestModel>("{id}")
    .UseRedis(context => context.GetStringRouteValue("id"));

// map post endpoint with validation
app.MapPostLayout<RedisTestModel>("{id}")
    .Validate(StatusCodes.Status400BadRequest, (context, model) => model.Y > 3)
    .UseRedis((context, body) => context.GetStringRouteValue("id"));

await app.RunAsync();
```

More info: https://github.com/Romfos/AutoEndpoints