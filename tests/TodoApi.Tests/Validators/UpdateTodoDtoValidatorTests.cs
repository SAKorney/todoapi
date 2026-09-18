using FluentAssertions;
using TodoApi.DTOs;
using TodoApi.Validators;
using Xunit;

namespace TodoApi.Tests.Validators;

public class UpdateTodoDtoValidatorTests
{
    private readonly UpdateTodoDtoValidator _validator = new();

    [Fact]
    public async Task Validate_ValidDto_IsValid()
    {
        var dto = new UpdateTodoDto("Updated task", true);
        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    public async Task Validate_InvalidTitle_IsInvalid(string? title)
    {
        var dto = new UpdateTodoDto(title!, false);
        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTodoDto.Title));
    }

    [Fact]
    public async Task Validate_TitleTooLong_IsInvalid()
    {
        var dto = new UpdateTodoDto(new string('Y', 201), true);
        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
    }
}
