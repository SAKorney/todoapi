using Microsoft.EntityFrameworkCore;
using TodoApi.Domain;
using TodoApi.Repositories;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddScoped<ITodoRepository, DbContextRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

//app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
    if (!await context.Items.AnyAsync())
    {
        var now = DateTime.UtcNow;
        var todos = Enumerable.Range(1, 10).Select(x => new TodoItem
        {
            Id = Guid.NewGuid(),
            IsCompleted = x % 2 == 0,
            CreatedAt = now.AddDays(-x),
            Title = $"Title {x}"
        });
        context.Items.AddRange(todos);
        await context.SaveChangesAsync();
    }
}

app.Run();
