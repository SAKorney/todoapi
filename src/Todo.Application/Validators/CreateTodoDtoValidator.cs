using FluentValidation;
using Todo.Application.DTOs;

namespace Todo.Application.Validators;

public class CreateTodoDtoValidator : TodoValidator<CreateTodoDto>
{
    public CreateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(MinLength, MaxLength).WithMessage(TitleLengthErrorMsg);
    }
}
