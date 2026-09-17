using AutoMapper;
using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;

namespace TodoApi.Services;

public class TodoService(ITodoRepository repository, ILogger<TodoService> logger, IMapper mapper, TimeProvider timeProvider) : ITodoService
{
    private readonly ITodoRepository _repository = repository;
    private readonly ILogger<TodoService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<IEnumerable<TodoResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get All Todos");
        var todos = await _repository.GetAllAsync(cancellationToken);
        return todos.Select(_mapper.Map<TodoResponseDto>);
    }

    public async Task<TodoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Get todo by id: {id}");
        var todo = await _repository.GetByIdAsync(id, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        return _mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<TodoResponseDto> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Create todo: {item}");
        var todo = TodoItem.Create(item.Title, _timeProvider.GetUtcNow().UtcDateTime);
        await _repository.AddAsync(todo, cancellationToken);
        _logger.LogInformation($"Todo {todo.Id} was created");
        return _mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<TodoResponseDto?> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Update todo {id} : {item}");
        var todo = await _repository.UpdateAsync(id, item.Title, item.IsCompleted, cancellationToken);
        if (todo is null)
        {
            return null;
        }
        _logger.LogInformation($"Todo {todo.Id} was updated");
        return _mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Delete todo {id}");
        return await _repository.DeleteAsync(id, cancellationToken);
    }
}
