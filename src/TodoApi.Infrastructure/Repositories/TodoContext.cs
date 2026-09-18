using Microsoft.EntityFrameworkCore;
using TodoApi.Domain;
using TodoApi.Repositories.Configurations;

namespace TodoApi.Repositories;

public class TodoContext : DbContext
{
    public DbSet<TodoItem> Items { get; set; } = null!;

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TodoItemConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
