using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validator;

public class TodoResponseDtoValidator : AbstractValidator<TodoResponseDto>
{
    private static readonly int _minLength = 2;
    private static readonly int _maxLength = 200;

    public TodoResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(_minLength, _minLength)
            .WithMessage($"Title must be between {_minLength} and {_maxLength} characters");
    }
}
