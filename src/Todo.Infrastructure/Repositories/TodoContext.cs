using Microsoft.EntityFrameworkCore;
using Todo.Domain;
using Todo.Infrastructure.Repositories.Configurations;

namespace Todo.Infrastructure.Repositories;

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
