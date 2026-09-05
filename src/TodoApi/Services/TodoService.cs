using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TodoResponseDto> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken);
    Task<TodoResponseDto?> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var todos = await _repository.GetAllAsync(cancellationToken);
        return todos.Select(x => new TodoResponseDto(x.Id, x.Title, x.IsCompleted, x.CreatedAt));
    }

    public async Task<TodoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(id, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        return new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<TodoResponseDto> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = item.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(todo, cancellationToken);

        return new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<TodoResponseDto?> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = await _repository.UpdateAsync(id, item.Title, item.IsCompleted, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        return new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }
}
