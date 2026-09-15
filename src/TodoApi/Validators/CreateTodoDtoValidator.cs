using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validator;

public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
{
    private static readonly int _minLength = 2;
    private static readonly int _maxLength = 200;

    public CreateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(_minLength, _minLength)
            .WithMessage($"Title must be between {_minLength} and {_maxLength} characters");
    }
}
