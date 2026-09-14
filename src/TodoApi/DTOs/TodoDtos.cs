using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs;

// Контракты для API
public sealed record CreateTodoDto(
    [property: Required(ErrorMessage = "Title is required")]
    [property: StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 2 and 200 characters")]
    string Title);

public sealed record UpdateTodoDto(
    [property: Required(ErrorMessage = "Title is required")]
    [property: StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 2 and 200 characters")]
    string Title,
    bool IsCompleted);

public sealed record TodoResponseDto(
    [property: Required(ErrorMessage = "Id is required")]
    Guid Id,
    [property: Required(ErrorMessage = "Title is required")]
    [property: StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 2 and 200 characters")]
    string Title,
    bool IsCompleted,
    DateTime CreatedAt);
