using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Scalar.AspNetCore;
using Todo.Application.DTOs;
using Todo.Application.Repositories;
using Todo.Application.Services;
using Todo.Domain;
using Todo.Infrastructure.Repositories;
using Todo.WebApi.Extensions;
using Todo.WebApi.Filters;
using Todo.WebApi.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);
builder.AddFlexibleLogging();

// DI
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ITodoRepository, DbContextRepository>();
builder.Services.AddScoped<ITodoService, TodoService>()
    .Decorate<ITodoService, TodoServiceLogger>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.AddValidatorsFromAssembly(typeof(TodoService).Assembly);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<LogActionFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
    await context.Database.EnsureCreatedAsync();
    if (!await context.Items.AnyAsync())
    {
        var now = DateTime.UtcNow;
        var todos = Enumerable.Range(1, 10)
            .Select((x, v) => (x, TodoItem.Create($"Title {x}", now.AddDays(-x))))
            .Select(t => { t.Item2.IsCompleted = t.x % 2 == 0; return t.Item2; });
        context.Items.AddRange(todos);
        await context.SaveChangesAsync();
    }
}

app.Logger.LogInformation("Application is starting, logging provider selected from configuration");
app.Run();

// Для интеграционных тестах (WebApplicationFactory)
public partial class Program;