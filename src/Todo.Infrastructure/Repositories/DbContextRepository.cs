using Microsoft.EntityFrameworkCore;
using Todo.Application.Repositories;
using Todo.Domain;

namespace Todo.Infrastructure.Repositories;

public class DbContextRepository(TodoContext context) : ITodoRepository
{
    public async Task AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        await context.Items.AddAsync(item, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var affected = await context.Items
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        return affected > 0;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Items
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TodoItem?> UpdateAsync(Guid id, string title, bool isCompleted, CancellationToken cancellationToken)
    {
        var existing = await context.Items.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Title = title;
        existing.IsCompleted = isCompleted;

        await context.SaveChangesAsync(cancellationToken);
        return existing;
    }
}
