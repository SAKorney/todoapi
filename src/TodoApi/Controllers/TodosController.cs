using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers;

[Route("api/v2/[controller]")]
[ApiController]
public class TodosController(ITodoService service, ILogger<TodosController> logger) : ControllerBase
{
    private readonly ITodoService _service = service;
    private readonly ILogger<TodosController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request all todos");
        var response = await _service.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))] // Начиная с версии 3.0 среда выполнения по умолчанию удаляет суффикс Async из имен экшенов при генерации маршрутов
    public async Task<ActionResult<TodoResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Request todo {id}");
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Request creation: {item}");
        var todo = await _service.CreateAsync(item, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoResponseDto>> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Request updating: {id}/{item}");
        var updated = await _service.UpdateAsync(id, item, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Request updating: {id}");
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            return NoContent();
        }
        return NotFound();
    }
}
