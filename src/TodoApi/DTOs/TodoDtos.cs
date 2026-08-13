namespace TodoApi.DTOs;

// Контракты для API
public record CreateTodoDto(string Title);
public record UpdateTodoDto(string Title, bool IsCompleted);
public record TodoResponseDto(Guid Id, string Title, bool IsCompleted, DateTime CreatedAt);
