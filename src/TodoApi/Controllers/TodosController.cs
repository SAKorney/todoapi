using System;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class TodosAsyncController(ITodoService service) : ControllerBase
    {
        private readonly ITodoService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _service.GetAll(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetById(id, cancellationToken);
            if (item is null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTodoDto item, CancellationToken cancellationToken)
        {
            var todo = await _service.Create(item, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
        {
            var existingTodo = await _service.GetById(id, cancellationToken);
            if (existingTodo is null)
            {
                return NotFound();
            }

            var updated = await _service.Update(id, item, cancellationToken);

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сохранения данных");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _service.Delete(id, cancellationToken);
            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
