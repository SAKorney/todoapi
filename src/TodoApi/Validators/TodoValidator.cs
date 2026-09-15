using FluentValidation;

namespace TodoApi.Validator;

public class ToDoValidator<T> : AbstractValidator<T>
{
    protected int MinLength => 2;
    protected int MaxLength => 200;
    protected string TitleLengthErrorMsg => $"Title must be between {MinLength} and {MaxLength} characters";
}
