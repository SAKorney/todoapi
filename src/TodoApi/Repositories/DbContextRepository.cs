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

    public async Task Add(TodoItem item, CancellationToken cancellationToken)
    {
        await _context.Items.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        var item = await _context.Items.FindAsync(new object[] { id }, cancellationToken);

        if (item is null)
        {
            return false;
        }

        _context.Items.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<TodoItem>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Items
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> Update(Guid id, string title, bool isCompleted, CancellationToken cancellationToken)
    {
        var existing = await _context.Items.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Title = title;
        existing.IsCompleted = isCompleted;

        var updatedRows = await _context.SaveChangesAsync(cancellationToken);
        return updatedRows > 0;
    }
}
