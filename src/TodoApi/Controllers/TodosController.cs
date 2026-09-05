using System;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class TodosController(ITodoService service) : ControllerBase
    {
        private readonly ITodoService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetAllAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
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
            var todo = await _service.CreateAsync(item, cancellationToken);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
        {
            var existingTodo = await _service.GetByIdAsync(id, cancellationToken);
            if (existingTodo is null)
            {
                return NotFound();
            }

            var updated = await _service.UpdateAsync(id, item, cancellationToken);

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сохранения данных");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
