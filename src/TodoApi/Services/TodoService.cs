using TodoApi.Domain;
using TodoApi.Repositories;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetAll();
    Task<TodoItem?> GetById(Guid id);
    Task<TodoItem> Create(string title);
    Task<bool> Update(Guid id, string title, bool isCompleted);
    Task<bool> Delete(Guid id);
}

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoItem>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<TodoItem?> GetById(Guid id)
    {
        return await _repository.GetById(id);
    }

    public async Task<TodoItem> Create(string title)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.Add(todo);
        return todo;
    }

    public async Task<bool> Update(Guid id, string title, bool isCompleted)
    {
        return await _repository.Update(new TodoItem
        {
            Id = id,
            Title = title,
            IsCompleted = isCompleted
        });
    }

    public async Task<bool> Delete(Guid id)
    {
        return await _repository.Delete(id);
    }
}
