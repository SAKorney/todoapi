using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs;

// Контракты для API
public record CreateTodoDto(
    [property: Required(ErrorMessage = "Title is required")]
    [property: StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 2 and 200 characters")]
    string Title);

public record UpdateTodoDto(
    [property: Required(ErrorMessage = "Title is required")]
    [property: StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 2 and 200 characters")]
    string Title,
    bool IsCompleted);

public record TodoResponseDto(
    [property: Required(ErrorMessage = "Id is required")]
    Guid Id,
    [property: Required(ErrorMessage = "Title is required")]
    [property: StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 2 and 200 characters")]
    string Title,
    bool IsCompleted,
    DateTime CreatedAt);
