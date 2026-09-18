using FluentValidation;
using Todo.Application.DTOs;

namespace Todo.Application.Validators;

public class UpdateTodoDtoValidator : TodoValidator<UpdateTodoDto>
{
    public UpdateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(MinLength, MaxLength).WithMessage(TitleLengthErrorMsg);
    }
}
