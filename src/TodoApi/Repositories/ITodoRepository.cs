using TodoApi.Domain;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    IEnumerable<TodoItem> GetAll();
    TodoItem? GetById(Guid id);
    void Add(TodoItem item);
    bool Update(TodoItem item);
    bool Delete(Guid id);
}

public interface ITodoRepositoryAsync
{
    Task<IEnumerable<TodoItem>> GetAll();
    Task<TodoItem?> GetById(Guid id);
    Task Add(TodoItem item);
    Task<bool> Update(TodoItem item);
    Task<bool> Delete(Guid id);
}
