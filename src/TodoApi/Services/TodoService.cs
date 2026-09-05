using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoResponseDto>> GetAll(CancellationToken cancellationToken);
    Task<TodoResponseDto?> GetById(Guid id, CancellationToken cancellationToken);
    Task<TodoResponseDto> Create(CreateTodoDto item, CancellationToken cancellationToken);
    Task<bool> Update(Guid id, UpdateTodoDto item, CancellationToken cancellationToken);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken);
}

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoResponseDto>> GetAll(CancellationToken cancellationToken)
    {
        var todos = await _repository.GetAll(cancellationToken);
        return todos.Select(x => new TodoResponseDto(x.Id, x.Title, x.IsCompleted, x.CreatedAt));
    }

    public async Task<TodoResponseDto?> GetById(Guid id, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetById(id, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        return new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<TodoResponseDto> Create(CreateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = item.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.Add(todo, cancellationToken);

        return new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<bool> Update(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        return await _repository.Update(id, item.Title, item.IsCompleted, cancellationToken);
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.Delete(id, cancellationToken);
    }
}
