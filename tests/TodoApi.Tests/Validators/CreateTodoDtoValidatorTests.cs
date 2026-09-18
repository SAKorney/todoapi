using FluentAssertions;
using TodoApi.DTOs;
using TodoApi.Validators;
using Xunit;

namespace TodoApi.Tests.Validators;

public class CreateTodoDtoValidatorTests
{
    private readonly CreateTodoDtoValidator _validator = new();

    [Theory]
    [InlineData("Valid title")]
    [InlineData("AB")]
    public async Task Validate_ValidTitle_IsValid(string title)
    {
        var dto = new CreateTodoDto(title);
        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_TitleExactlyMaxLength_IsValid()
    {
        var dto = new CreateTodoDto(new string('A', 200));
        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public async Task Validate_InvalidTitle_IsInvalid(string title)
    {
        var dto = new CreateTodoDto(title);
        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTodoDto.Title));
    }

    [Fact]
    public async Task Validate_TitleTooLong_IsInvalid()
    {
        var dto = new CreateTodoDto(new string('X', 201));
        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTodoDto.Title));
    }
}
