using FluentValidation;
using Todo.Application.DTOs;

namespace Todo.Application.Validators;

public class TodoQueryParametersValidator : TodoValidator<TodoQueryParameters>
{
    private static readonly int _minCount = 1;
    public TodoQueryParametersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(_minCount)
            .WithMessage($"Page must be >= {_minCount}");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(_minCount)
            .WithMessage($"PageSize must be >= {_minCount}");
    }
}