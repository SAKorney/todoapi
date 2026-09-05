using System;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;
using TodoApi.Services;

namespace TodoApi
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class TodosAsyncController(ITodoService service) : ControllerBase
    {
        private readonly ITodoService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetAll()
        {
            var items = await _service.GetAll();
            var response = items.Select(x => new TodoResponseDto(x.Id, x.Title, x.IsCompleted, x.CreatedAt));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponseDto>> GetById(Guid id)
        {
            var item = await _service.GetById(id);
            if (item is null)
            {
                return NotFound();
            }
            var response = new TodoResponseDto(item.Id, item.Title, item.IsCompleted, item.CreatedAt);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTodoDto item)
        {
            var todo = await _service.Create(item.Title);

            var response = new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);

            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTodoDto item)
        {
            var existingTodo = await _service.GetById(id);
            if (existingTodo is null)
            {
                return NotFound();
            }

            var updated = await _service.Update(id, item.Title, item.IsCompleted);

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сохранения данных");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.Delete(id);
            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
