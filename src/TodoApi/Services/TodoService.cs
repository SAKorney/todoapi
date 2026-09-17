using AutoMapper;
using TodoApi.Domain;
using TodoApi.DTOs;
using TodoApi.Repositories;

namespace TodoApi.Services;

public class TodoService(ITodoRepository repository, IMapper mapper, TimeProvider timeProvider) : ITodoService
{
    private readonly ITodoRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<IEnumerable<TodoResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var todos = await _repository.GetAllAsync(cancellationToken);
        return todos.Select(_mapper.Map<TodoResponseDto>);
    }

    public async Task<TodoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(id, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        return _mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<TodoResponseDto> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = TodoItem.Create(item.Title, _timeProvider.GetUtcNow().UtcDateTime);
        await _repository.AddAsync(todo, cancellationToken);
        return _mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<TodoResponseDto?> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken)
    {
        var todo = await _repository.UpdateAsync(id, item.Title, item.IsCompleted, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        return _mapper.Map<TodoResponseDto>(todo);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }
}
