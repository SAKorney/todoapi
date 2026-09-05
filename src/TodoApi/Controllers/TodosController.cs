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
            var response = await _service.GetAll();
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
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTodoDto item)
        {
            var todo = await _service.Create(item);

            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTodoDto item)
        {
            var existingTodo = await _service.GetById(id);
            if (existingTodo is null)
            {
                return NotFound();
            }

            var updated = await _service.Update(id, item);

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
