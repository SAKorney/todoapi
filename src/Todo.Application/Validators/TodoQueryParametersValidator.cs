using FluentValidation;
using Todo.Application.DTOs;

namespace Todo.Application.Validators;

public class TodoQueryParametersValidator : TodoValidator<TodoQueryParameters>
{
    private static readonly int _minCount = 1;
    
    private static readonly int _maxCount = 100;
    public TodoQueryParametersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(_minCount)
            .WithMessage($"Page must be >= {_minCount}");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(_minCount, _maxCount)
            .WithMessage($"PageSize must be between {_minCount} asn {_maxCount}");
    }
}