using FluentAssertions;
using Todo.Application.DTOs;
using Todo.Application.Validators;
using Xunit;

namespace Todo.Tests.Validators;

public class TodoQueryParametersValidatorTests
{
    private readonly TodoQueryParametersValidator _validator = new();

    [Fact]
    public async Task Validate_DefaultParameters_IsValid()
    {
        var query = new TodoQueryParameters();
        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(5, 100)]
    public async Task Validate_NonNegativePageAndPageSize_IsValid(int page, int pageSize)
    {
        var query = new TodoQueryParameters(Page: page, PageSize: pageSize);
        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public async Task Validate_ZeroPageAndPageSize_IsInvalid(int page, int pageSize)
    {
        var query = new TodoQueryParameters(Page: page, PageSize: pageSize);
        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task Validate_NegativePage_IsInvalid(int page)
    {
        var query = new TodoQueryParameters(Page: page, PageSize: 10);
        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(TodoQueryParameters.Page) &&
            e.ErrorMessage.Contains("Page must be >="));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public async Task Validate_NegativePageSize_IsInvalid(int pageSize)
    {
        var query = new TodoQueryParameters(Page: 1, PageSize: pageSize);
        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(TodoQueryParameters.PageSize) &&
            e.ErrorMessage.Contains("PageSize must be >="));
    }

    [Fact]
    public async Task Validate_BothPageAndPageSizeNegative_ReturnsErrorsForBoth()
    {
        var query = new TodoQueryParameters(Page: -1, PageSize: -1);
        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(TodoQueryParameters.Page));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(TodoQueryParameters.PageSize));
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task Validate_OtherPropertiesDoNotAffectValidity()
    {
        // SortBy / Search / IsCompleted не валидируются этим валидатором
        var query = new TodoQueryParameters(
            Page: 1,
            PageSize: 10,
            IsCompleted: true,
            Search: "anything",
            SortBy: "unknownField",
            SortDir: "whatever");

        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }
}
