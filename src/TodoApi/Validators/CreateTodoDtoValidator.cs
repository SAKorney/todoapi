using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validator;

public class CreateTodoDtoValidator : ToDoValidator<CreateTodoDto>
{
    public CreateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(MinLength, MaxLength).WithMessage(TitleLengthErrorMsg);
    }
}
