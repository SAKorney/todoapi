using Microsoft.EntityFrameworkCore;
using Todo.Application.DTOs;
using Todo.Application.Repositories;
using Todo.Domain;

namespace Todo.Infrastructure.Repositories;

public class DbContextRepository(TodoContext context) : ITodoRepository
{
    public async Task<PagedResult<TodoItem>> GetPagedAsync(
        TodoQueryParameters query,
        CancellationToken cancellationToken)
    {
        var q = context.Items.AsNoTracking().AsQueryable();

        if (query.IsCompleted is not null)
            q = q.Where(x => x.IsCompleted == query.IsCompleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(x => x.Title.Contains(term));
        }

        var totalCount = await q.CountAsync(cancellationToken);

        q = (query.SortBy.ToLowerInvariant(), query.SortDir.ToLowerInvariant()) switch
        {
            ("title", "asc") => q.OrderBy(x => x.Title),
            ("title", _) => q.OrderByDescending(x => x.Title),
            ("iscompleted", "asc") => q.OrderBy(x => x.IsCompleted),
            ("iscompleted", _) => q.OrderByDescending(x => x.IsCompleted),
            ("createdat", "asc") => q.OrderBy(x => x.CreatedAt),
            _ => q.OrderByDescending(x => x.CreatedAt)
        };

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResult<TodoItem>(items, totalCount, query.Page, query.PageSize);
    }

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
