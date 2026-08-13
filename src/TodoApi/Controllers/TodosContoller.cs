using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;

namespace TodoApi
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TodosContoller(ITodoRepository repository) : ControllerBase
    {
        private readonly ITodoRepository _repository = repository;

        [HttpGet]
        public ActionResult<IEnumerable<TodoResponseDto>> GetAll()
        {
            var response = _repository.GetAll().Select(x => new TodoResponseDto(x.Id, x.Title, x.IsCompleted, x.CreatedAt));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public ActionResult<TodoResponseDto> GetById(Guid id)
        {
            var item = _repository.GetById(id);
            if (item is null)
            {
                return NotFound();
            }
            var response = new TodoResponseDto(item.Id, item.Title, item.IsCompleted, item.CreatedAt);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Create(CreateTodoDto item)
        {
            var todo = new TodoItem()
            {
                Id = Guid.NewGuid(),
                Title = item.Title,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Add(todo);

            var response = new TodoResponseDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);

            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UpdateTodoDto item)
        {
            var existingTodo = _repository.GetById(id);
            if (existingTodo is null)
            {
                return NotFound();
            }

            existingTodo.Title = item.Title;
            existingTodo.IsCompleted = item.IsCompleted;

            _repository.Update(existingTodo);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var deleted = _repository.Delete(id);
            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
