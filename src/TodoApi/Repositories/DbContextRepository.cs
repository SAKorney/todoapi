using Microsoft.EntityFrameworkCore;
using TodoApi.Domain;

namespace TodoApi.Repositories;

public class DbContextRepository : ITodoRepositoryAsync
{
    private readonly TodoContext _context;

    public DbContextRepository(TodoContext context)
    {
        _context = context;
    }

    public async Task Add(TodoItem item)
    {
        await _context.Items.AddAsync(item);
    }

    public async Task<bool> Delete(Guid id)
    {
        var item = await _context.Items.FindAsync(id);

        if (item is null)
        {
            return false;
        }

        _context.Items.Remove(item);
        return true;
    }

    public async Task<IEnumerable<TodoItem>> GetAll()
    {
        return await _context.Items.AsNoTracking().ToListAsync();
    }

    public async Task<TodoItem?> GetById(Guid id)
    {
        return await _context.Items.FindAsync(id);
    }

    public async Task<bool> Update(TodoItem item)
    {
        _context.Items.Update(item);

        if (!_context.ChangeTracker.HasChanges())
        {
            return false;
        }

        var res = await _context.SaveChangesAsync();
        return res > 0;
    }
}
