using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Todo.Infrastructure.Repositories;

namespace Todo.Tests.Infrastructure;

/// <summary>
/// WebApplicationFactory с SQLite in-memory.
/// Connection держится открытым на всё время жизни фабрики —
/// иначе :memory: БД уничтожается при закрытии connection.
/// </summary>
public class TodoWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public TodoWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            RemoveDbContextRegistrations<TodoContext>(services);

            services.AddDbContext<TodoContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    private static void RemoveDbContextRegistrations<TContext>(IServiceCollection services)
           where TContext : DbContext
    {
        var toRemove = services
            .Where(d =>
                d.ServiceType == typeof(TContext) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(DbContextOptions<TContext>) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>) &&
                 d.ServiceType.GenericTypeArguments[0] == typeof(TContext)))
            .ToList();

        foreach (var descriptor in toRemove)
        {
            services.Remove(descriptor);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
