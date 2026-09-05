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
    }
}
