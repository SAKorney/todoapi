using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validator;

public class UpdateTodoDtoValidator : ToDoValidator<UpdateTodoDto>
{
    public UpdateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(MinLength, MaxLength).WithMessage(TitleLengthErrorMsg);
    }
}
