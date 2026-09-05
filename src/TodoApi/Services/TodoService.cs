using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoResponseDto>> GetAll();
    Task<TodoResponseDto?> GetById(Guid id);
    Task<TodoResponseDto> Create(CreateTodoDto item);
    Task<bool> Update(Guid id, UpdateTodoDto item);
    Task<bool> Delete(Guid id);
}

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoResponseDto>> GetAll()
    {
        var todos = await _repository.GetAll();
        return todos.Select(x => new TodoResponseDto(x.Id, x.Title, x.IsCompleted, x.CreatedAt));
    }

    public async Task<TodoResponseDto?> GetById(Guid id)
    {
        var todo = await _repository.GetById(id);
        if (todo is null)
        {
            return null;
        }

        return new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<TodoResponseDto> Create(CreateTodoDto item)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = item.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.Add(todo);

        return new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<bool> Update(Guid id, UpdateTodoDto item)
    {
        return await _repository.Update(id, item.Title, item.IsCompleted);
    }

    public async Task<bool> Delete(Guid id)
    {
        return await _repository.Delete(id);
    }
}
