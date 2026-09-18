using FluentValidation;
using TodoApi.Domain;

namespace TodoApi.Validators;

public class TodoValidator<T> : AbstractValidator<T>
{
    protected int MinLength => TodoItemConstraints.TitleMinLength;
    protected int MaxLength => TodoItemConstraints.TitleMaxLength;
    protected string TitleLengthErrorMsg => $"Title must be between {MinLength} and {MaxLength} characters";
}
