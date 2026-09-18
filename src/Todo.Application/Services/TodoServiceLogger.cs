using Microsoft.Extensions.Logging;
using Todo.Application.DTOs;

namespace Todo.Application.Services;

public class TodoServiceLogger(ITodoService todoService, ILogger<TodoServiceLogger> logger) : ITodoService
{
     public async Task<PagedResult<TodoResponseDto>> GetPagedAsync(
        TodoQueryParameters query,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching paged todos: Page={Page}, PageSize={PageSize}, IsCompleted={IsCompleted}, Search={Search}, SortBy={SortBy}, SortDir={SortDir}",
            query.Page, query.PageSize, query.IsCompleted, query.Search, query.SortBy, query.SortDir);
        try
        {
            var result = await todoService.GetPagedAsync(query, cancellationToken);
            logger.LogInformation(
                "Successfully fetched {Count} of {TotalCount} todo items (page {Page}/{TotalPages})",
                result.Items.Count, result.TotalCount, result.Page, result.TotalPages);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching paged todo items");
            throw;
        }
    }

    public async Task<TodoResponseDto> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating a new todo item: {@Item}", item);
        try
        {
            var result = await todoService.CreateAsync(item, cancellationToken);
            logger.LogInformation("Todo item created successfully with ID: {Id}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating todo item");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting todo item with ID: {Id}", id);
        try
        {
            var deleted = await todoService.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                logger.LogInformation("Todo item with ID: {Id} deleted successfully", id);
            }
            else
            {
                logger.LogWarning("Todo item with ID: {Id} not found for deletion", id);
            }
            return deleted;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting todo item with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TodoResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all todo items");
        try
        {
            var items = await todoService.GetAllAsync(cancellationToken);
            logger.LogInformation("Successfully fetched todo items");
            return items;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching all todo items");
            throw;
        }
    }

    public async Task<TodoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching todo item with ID: {Id}", id);
        try
        {
            var item = await todoService.GetByIdAsync(id, cancellationToken);
            if (item == null)
            {
                logger.LogWarning("Todo item with ID: {Id} not found", id);
            }
            else
            {
                logger.LogInformation("Successfully fetched todo item with ID: {Id}", id);
            }
            return item;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching todo item with ID: {Id}", id);
            throw;
        }
    }

 public async Task<TodoResponseDto?> UpdateStatusAsync(
        Guid id,
        UpdateTodoStatusDto item,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Setting completion status for todo ID: {Id} to {IsCompleted}",
            id, item.IsCompleted);
        try
        {
            var result = await todoService.UpdateStatusAsync(id, item, cancellationToken);
            if (result is null)
            {
                logger.LogWarning("Todo item with ID: {Id} not found for status update", id);
            }
            else
            {
                logger.LogInformation(
                    "Todo item with ID: {Id} completion status set to {IsCompleted}",
                    id, result.IsCompleted);
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while setting completion status for todo ID: {Id}", id);
            throw;
        }
    }

    public async Task<TodoResponseDto?> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating todo item with ID: {Id}. New data: {@Item}", id, item);
        try
        {
            var result = await todoService.UpdateAsync(id, item, cancellationToken);
            if (result == null)
            {
                logger.LogWarning("Todo item with ID: {Id} not found for update", id);
            }
            else
            {
                logger.LogInformation("Todo item with ID: {Id} updated successfully", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating todo item with ID: {Id}", id);
            throw;
        }
    }
}
