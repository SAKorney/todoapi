using TodoApi.Domain;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAll();
    Task<TodoItem?> GetById(Guid id);
    Task Add(TodoItem item);
    Task<bool> Update(TodoItem item);
    Task<bool> Delete(Guid id);
}
