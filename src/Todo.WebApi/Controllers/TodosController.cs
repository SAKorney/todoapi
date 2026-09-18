using Microsoft.AspNetCore.Mvc;
using Todo.Application.DTOs;
using Todo.Application.Services;

namespace Todo.WebApi.Controllers;

[Route("api/v2/[controller]")]
[ApiController]
public class TodosController(ITodoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<TodoResponseDto>>> GetAllAsync(
        [FromQuery] TodoQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))] // Начиная с версии 3.0 среда выполнения по умолчанию удаляет суффикс Async из имен экшенов при генерации маршрутов
    public async Task<ActionResult<TodoResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await service.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = await service.CreateAsync(item, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoResponseDto>> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        var updated = await service.UpdateAsync(id, item, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<TodoResponseDto>> UpdateStatusAsync(
        Guid id,
        UpdateTodoStatusDto item,
        CancellationToken cancellationToken)
    {
        var updated = await service.UpdateStatusAsync(id, item, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            return NoContent();
        }
        return NotFound();
    }
}
