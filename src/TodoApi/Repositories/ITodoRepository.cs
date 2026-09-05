using TodoApi.Domain;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAll(CancellationToken cancellationToken);
    Task<TodoItem?> GetById(Guid id, CancellationToken cancellationToken);
    Task Add(TodoItem item, CancellationToken cancellationToken);
    Task<bool> Update(Guid id, string title, bool isCompleted, CancellationToken cancellationToken);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken);
}
