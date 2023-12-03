using Microsoft.EntityFrameworkCore;

namespace AutoEndpoints.SqlServer.App;

// need to use required keyword for .net 7+
#nullable disable

public sealed class SqlServerContext(DbContextOptions<SqlServerContext> options) : DbContext(options)
{
    public DbSet<SqlServerTestModel> SqlServerTestModels { get; set; }
}