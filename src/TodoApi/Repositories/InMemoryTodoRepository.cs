using System.Collections.Concurrent;
using TodoApi.Domain;

namespace TodoApi.Repositories;

public class InMemoryTodoRepository : ITodoRepository
{
    // Thread-safety
    private readonly ConcurrentDictionary<Guid, TodoItem> _storage = new();

    public void Add(TodoItem item) => _storage.TryAdd(item.Id, item);

    public bool Delete(Guid id) => _storage.TryRemove(id, out _);

    public IEnumerable<TodoItem> GetAll() => _storage.Values.OrderBy(x => x.CreatedAt);

    public TodoItem? GetById(Guid id)
    {
        _storage.TryGetValue(id, out var item);
        return item;
    }

    public bool Update(TodoItem item)
    {
        var id = item.Id;
        if (_storage.ContainsKey(id))
        {
            _storage[id] = item;
            return true;
        }

        return false;
    }
}
