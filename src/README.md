# Overview

AutoEndpoints is ASP.NET Core based framework for rapid WebAPI development.
Allow directly map database entities to web api endpoints with access verification and body validation

Supported databases:
- [Redis](https://www.nuget.org/packages/AutoEndpoints.Redis) 
- [Microsoft Azure Cosmos DB](https://www.nuget.org/packages/AutoEndpoints.Cosmos)
- [Microsoft SQL Server with dapper](https://www.nuget.org/packages/AutoEndpoints.Dapper.SqlServer)

Supported platforms:
 - .NET 6
 - .NET 7 
 - .NET 8

# Example

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedisEndpoints("localhost:6379");

var app = builder.Build();

app.MapRedisGetEndpoint<RedisTestModel>("{id}")
    .KeyFromRoute("id")
    .Build();

app.MapRedisPostEndpoint<RedisTestModel>("{id}")
    .KeyFromRoute("id")
    .Build();

await app.RunAsync();
```

More info: https://github.com/Romfos/AutoEndpoints