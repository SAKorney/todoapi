using AutoMapper;
using Todo.Application.DTOs;
using Todo.Application.Repositories;
using Todo.Domain;

namespace Todo.Application.Services;

public class TodoService(ITodoRepository repository, IMapper mapper, TimeProvider timeProvider) : ITodoService
{
    public async Task<PagedResult<TodoResponseDto>> GetPagedAsync(
        TodoQueryParameters query,
        CancellationToken cancellationToken)
    {
        var paged = await repository.GetPagedAsync(query, cancellationToken);
        var todos = paged.Items.Select(mapper.Map<TodoResponseDto>).ToList();
        return new PagedResult<TodoResponseDto>(todos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<TodoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await repository.GetByIdAsync(id, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        return mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<TodoResponseDto> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = TodoItem.Create(item.Title, timeProvider.GetUtcNow().UtcDateTime);
        await repository.AddAsync(todo, cancellationToken);
        return mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<TodoResponseDto?> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = await repository.UpdateAsync(id, item.Title, item.IsCompleted, cancellationToken);
        if (todo is null)
        {
            return null;
        }
        return mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<TodoResponseDto?> UpdateStatusAsync(
        Guid id,
        UpdateTodoStatusDto item,
        CancellationToken cancellationToken)
    {
        var todo = await repository.UpdateStatusAsync(id, item.IsCompleted, cancellationToken);
        if (todo is null)
        {
            return null;
        }
        return mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }
}
