using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validator;

public class TodoResponseDtoValidator : ToDoValidator<TodoResponseDto>
{
    public TodoResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(MinLength, MaxLength)
            .WithMessage(TitleLengthErrorMsg);
    }
}
