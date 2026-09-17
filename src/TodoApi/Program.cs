using Microsoft.EntityFrameworkCore;
using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;
using TodoApi.Services;
using TodoApi.Validators;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ITodoRepository, DbContextRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
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
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
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

app.Run();
