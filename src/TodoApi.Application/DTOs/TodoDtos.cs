using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs;

// Контракты для API
public sealed record CreateTodoDto(
    string Title);

public sealed record UpdateTodoDto(
    string Title,
    bool IsCompleted);

public sealed record TodoResponseDto(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTime CreatedAt);
