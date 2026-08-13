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
