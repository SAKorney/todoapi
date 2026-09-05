using System;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;

namespace TodoApi
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class TodosAsyncController(ITodoRepository repository) : ControllerBase
    {
        private readonly ITodoRepository _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetAll()
        {
            var items = await _repository.GetAll();
            var response = items.Select(x => new TodoResponseDto(x.Id, x.Title, x.IsCompleted, x.CreatedAt));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponseDto>> GetById(Guid id)
        {
            var item = await _repository.GetById(id);
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
            var todo = new TodoItem()
            {
                Id = Guid.NewGuid(),
                Title = item.Title,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.Add(todo);

            var response = new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);

            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTodoDto item)
        {
            var existingTodo = await _repository.GetById(id);
            if (existingTodo is null)
            {
                return NotFound();
            }

            existingTodo.Title = item.Title;
            existingTodo.IsCompleted = item.IsCompleted;

            var updated = await _repository.Update(existingTodo);

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сохранения данных");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _repository.Delete(id);
            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }

    }
}
