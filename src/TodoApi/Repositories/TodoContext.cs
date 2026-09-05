using System;
using Microsoft.EntityFrameworkCore;
using TodoApi.Domain;

namespace TodoApi.Repositories;

public class TodoContext : DbContext
{
    public DbSet<TodoItem> Items { get; set; } = null!;

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
        var now = DateTime.Now;
        var todos = Enumerable.Range(1, 10).Select(x => new TodoItem()
        {
            Id = Guid.NewGuid(),
            IsCompleted = x % 2 == 0,
            CreatedAt = now.AddDays(-x),
            Title = $"Title {x}"
        });

        Items.AddRange(todos);
    }
}
