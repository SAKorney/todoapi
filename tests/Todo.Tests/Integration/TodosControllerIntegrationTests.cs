using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Todo.Application.DTOs;
using Todo.Tests.Infrastructure;
using Xunit;

namespace Todo.Tests.Integration;

public class TodosControllerIntegrationTests : IClassFixture<TodoWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TodosControllerIntegrationTests(TodoWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region GET list

    [Fact]
    public async Task GetAll_ReturnsPagedResultWithSeedData()
    {
        var response = await _client.GetAsync("/api/v2/Todos?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<TodoResponseDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.TotalCount.Should().BeGreaterThan(0);
        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPageSize()
    {
        var response = await _client.GetAsync("/api/v2/Todos?page=1&pageSize=3");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<TodoResponseDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.PageSize.Should().Be(3);
    }

    [Fact]
    public async Task GetAll_FilterByIsCompleted_ReturnsOnlyMatching()
    {
        var response = await _client.GetAsync("/api/v2/Todos?isCompleted=true&pageSize=50");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<TodoResponseDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().OnlyContain(x => x.IsCompleted);
    }

    [Fact]
    public async Task GetAll_InvalidPage_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v2/Todos?page=0&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GET by id

    [Fact]
    public async Task GetById_WhenExists_ReturnsTodo()
    {
        var created = await CreateTodoAsync("Integration get-by-id");

        var response = await _client.GetAsync($"/api/v2/Todos/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponseDto>(JsonOptions);
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(created.Id);
        todo.Title.Should().Be("Integration get-by-id");
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/v2/Todos/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST

    [Fact]
    public async Task Create_ValidDto_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/v2/Todos", new CreateTodoDto("Created via integration test"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var todo = await response.Content.ReadFromJsonAsync<TodoResponseDto>(JsonOptions);
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Created via integration test");
        todo.IsCompleted.Should().BeFalse();
        todo.Id.Should().NotBeEmpty();
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_EmptyTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/v2/Todos", new CreateTodoDto(""));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_TitleTooShort_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/v2/Todos", new CreateTodoDto("A"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PUT

    [Fact]
    public async Task Update_WhenExists_ReturnsOk()
    {
        var created = await CreateTodoAsync("Before update");

        var response = await _client.PutAsJsonAsync(
            $"/api/v2/Todos/{created.Id}",
            new UpdateTodoDto("After update", true));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<TodoResponseDto>(JsonOptions);
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("After update");
        updated.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WhenNotExists_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/v2/Todos/{Guid.NewGuid()}",
            new UpdateTodoDto("Missing", false));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region PATCH status

    [Fact]
    public async Task UpdateStatus_MarkCompleted_ReturnsOk()
    {
        var created = await CreateTodoAsync("To complete");

        var response = await _client.PatchAsJsonAsync(
            $"/api/v2/Todos/{created.Id}/status",
            new UpdateTodoStatusDto(true));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<TodoResponseDto>(JsonOptions);
        updated.Should().NotBeNull();
        updated!.IsCompleted.Should().BeTrue();
        updated.Title.Should().Be("To complete");
    }

    [Fact]
    public async Task UpdateStatus_MarkIncomplete_ReturnsOk()
    {
        var created = await CreateTodoAsync("Was done");
        await _client.PatchAsJsonAsync(
            $"/api/v2/Todos/{created.Id}/status",
            new UpdateTodoStatusDto(true));

        var response = await _client.PatchAsJsonAsync(
            $"/api/v2/Todos/{created.Id}/status",
            new UpdateTodoStatusDto(false));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<TodoResponseDto>(JsonOptions);
        updated!.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateStatus_WhenNotExists_ReturnsNotFound()
    {
        var response = await _client.PatchAsJsonAsync(
            $"/api/v2/Todos/{Guid.NewGuid()}/status",
            new UpdateTodoStatusDto(true));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContent()
    {
        var created = await CreateTodoAsync("To delete");

        var response = await _client.DeleteAsync($"/api/v2/Todos/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v2/Todos/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenNotExists_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/v2/Todos/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    private async Task<TodoResponseDto> CreateTodoAsync(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/v2/Todos", new CreateTodoDto(title));
        response.EnsureSuccessStatusCode();
        var todo = await response.Content.ReadFromJsonAsync<TodoResponseDto>(JsonOptions);
        return todo!;
    }
}
