using FluentValidation;
using Todo.Domain;

namespace Todo.Application.Validators;

public class TodoValidator<T> : AbstractValidator<T>
{
    protected int MinLength => TodoItemConstraints.TitleMinLength;
    protected int MaxLength => TodoItemConstraints.TitleMaxLength;
    protected string TitleLengthErrorMsg => $"Title must be between {MinLength} and {MaxLength} characters";
}
