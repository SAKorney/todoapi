using TodoApi.Domain;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(TodoItem item, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, string title, bool isCompleted, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
