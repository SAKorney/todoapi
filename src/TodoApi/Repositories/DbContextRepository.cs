using Microsoft.EntityFrameworkCore;
using TodoApi.Domain;

namespace TodoApi.Repositories;

public class DbContextRepository : ITodoRepository
{
    private readonly TodoContext _context;

    public DbContextRepository(TodoContext context)
    {
        _context = context;
    }

    public async Task Add(TodoItem item)
    {
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Delete(Guid id)
    {
        var item = await _context.Items.FindAsync(id);

        if (item is null)
        {
            return false;
        }

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
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
        var existing = await _context.Items.FindAsync(item.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Title = item.Title;
        existing.IsCompleted = item.IsCompleted;

        var updatedRows = await _context.SaveChangesAsync();
        return updatedRows > 0;
    }
}
