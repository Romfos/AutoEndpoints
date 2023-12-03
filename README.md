# Overview

AutoEndpoints is a framework for automatic database mapping to asp.net core web api with verification and validation

[![.github/workflows/build.yml](https://github.com/Romfos/AutoEndpoints/actions/workflows/build.yml/badge.svg)](https://github.com/Romfos/AutoEndpoints/actions/workflows/build.yml)

[![AutoEndpoints.Redis](https://img.shields.io/nuget/v/AutoEndpoints.Redis?label=AutoEndpoints.Redis)](https://www.nuget.org/packages/AutoEndpoints.Redis)\
[![AutoEndpoints.Cosmos](https://img.shields.io/nuget/v/AutoEndpoints.Cosmos?label=AutoEndpoints.Cosmos)](https://www.nuget.org/packages/AutoEndpoints.Cosmos)\
[![AutoEndpoints.SqlServer](https://img.shields.io/nuget/v/AutoEndpoints.SqlServer?label=AutoEndpoints.SqlServer)](https://www.nuget.org/packages/AutoEndpoints.SqlServer)

Supported databases:
1) Redis
2) Microsoft Azure Cosmos DB
3) Microsoft SQL Server

Supported platforms:
 - .NET 6
 - .NET 7 
 - .NET 8

# Example

Program.cs:
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

# Nuget packages links  
- https://www.nuget.org/packages/AutoEndpoints.Redis
- https://www.nuget.org/packages/AutoEndpoints.Cosmos
- https://www.nuget.org/packages/AutoEndpoints.SqlServer

  
# How to use
Basic steps:
1) Create new web api project
2) Added nuget package fro target database
   
| Database                  | Nuget package                                                                     |
|---------------------------|-----------------------------------------------------------------------------------|
| Redis                     | [AutoEndpoints.Redis](https://www.nuget.org/packages/AutoEndpoints.Redis)         |
| Microsoft Azure Cosmos DB | [AutoEndpoints.Cosmos](https://www.nuget.org/packages/AutoEndpoints.Cosmos)       |
| Microsoft SQL Server      | [AutoEndpoints.SqlServer](https://www.nuget.org/packages/AutoEndpoints.SqlServer) |

3) Register database provider in services
```
builder.Services.AddRedisEndpoints("localhost:6379");
```
4) Map endpoints
```
app.MapRedisGetEndpoint<RedisTestModel>("{id}")
    .KeyFromRoute("id")
    .Build();
```
Done

