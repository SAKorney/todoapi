using AutoMapper;
using FluentAssertions;
using Moq;
using Todo.Application.DTOs;
using Todo.Application.Repositories;
using Todo.Application.Services;
using Todo.Domain;
using Xunit;

namespace Todo.Tests.Services;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly FakeTimeProvider _timeProvider;
    private readonly TodoService _sut;
    private readonly DateTime _fixedUtcNow = new(2026, 9, 18, 12, 0, 0, DateTimeKind.Utc);

    public TodoServiceTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _timeProvider = new FakeTimeProvider(_fixedUtcNow);
        _sut = new TodoService(_repositoryMock.Object, _mapperMock.Object, _timeProvider);
    }

    #region GetPagedAsync

    [Fact]
    public async Task GetPagedAsync_WhenItemsExist_ReturnsMappedPagedResult()
    {
        // Arrange
        var query = new TodoQueryParameters(Page: 1, PageSize: 10);
        var items = new List<TodoItem>
        {
            CreateTodoItem("Task 1"),
            CreateTodoItem("Task 2", isCompleted: true)
        };

        var pagedEntities = new PagedResult<TodoItem>(items, TotalCount: 2, Page: 1, PageSize: 10);

        _repositoryMock
            .Setup(r => r.GetPagedAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedEntities);

        foreach (var item in items)
        {
            _mapperMock
                .Setup(m => m.Map<TodoResponseDto>(item))
                .Returns(MapToDto(item));
        }

        // Act
        var result = await _sut.GetPagedAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
        result.Items.Should().BeEquivalentTo(items.Select(MapToDto));

        _repositoryMock.Verify(r => r.GetPagedAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WhenEmpty_ReturnsEmptyPagedResult()
    {
        // Arrange
        var query = new TodoQueryParameters();
        var pagedEntities = new PagedResult<TodoItem>([], TotalCount: 0, Page: 1, PageSize: 10);

        _repositoryMock
            .Setup(r => r.GetPagedAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedEntities);

        // Act
        var result = await _sut.GetPagedAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedAsync_WhenMultiplePages_ExposesPagingMetadata()
    {
        // Arrange
        var query = new TodoQueryParameters(Page: 2, PageSize: 5);
        var items = new List<TodoItem>
        {
            CreateTodoItem("Task 6"),
            CreateTodoItem("Task 7")
        };

        // totalCount=12, pageSize=5 → 3 pages; page 2 has next and previous
        var pagedEntities = new PagedResult<TodoItem>(items, TotalCount: 12, Page: 2, PageSize: 5);

        _repositoryMock
            .Setup(r => r.GetPagedAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedEntities);

        foreach (var item in items)
        {
            _mapperMock
                .Setup(m => m.Map<TodoResponseDto>(item))
                .Returns(MapToDto(item));
        }

        // Act
        var result = await _sut.GetPagedAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(12);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(3);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetPagedAsync_PassesQueryParametersToRepository()
    {
        // Arrange
        var query = new TodoQueryParameters(
            Page: 1,
            PageSize: 20,
            IsCompleted: false,
            Search: "buy",
            SortBy: "title",
            SortDir: "asc");

        var pagedEntities = new PagedResult<TodoItem>([], TotalCount: 0, Page: 1, PageSize: 20);

        _repositoryMock
            .Setup(r => r.GetPagedAsync(It.IsAny<TodoQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedEntities);

        // Act
        await _sut.GetPagedAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.GetPagedAsync(
                It.Is<TodoQueryParameters>(q =>
                    q.Page == 1 &&
                    q.PageSize == 20 &&
                    q.IsCompleted == false &&
                    q.Search == "buy" &&
                    q.SortBy == "title" &&
                    q.SortDir == "asc"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_WhenItemsExist_ReturnsMappedDtos()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            CreateTodoItem("Task 1"),
            CreateTodoItem("Task 2", isCompleted: true)
        };

        var expectedDtos = items.Select(MapToDto).ToList();

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        foreach (var item in items)
        {
            _mapperMock
                .Setup(m => m.Map<TodoResponseDto>(item))
                .Returns(MapToDto(item));
        }

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedDtos);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<TodoItem>());

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsMappedDto()
    {
        // Arrange
        var item = CreateTodoItem("Existing task");
        var expectedDto = MapToDto(item);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mapperMock
            .Setup(m => m.Map<TodoResponseDto>(item))
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetByIdAsync(item.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _sut.GetByIdAsync(id, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mapperMock.Verify(m => m.Map<TodoResponseDto>(It.IsAny<TodoItem>()), Times.Never);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ValidDto_CreatesItemAndReturnsDto()
    {
        // Arrange
        var createDto = new CreateTodoDto("New task");
        TodoItem? capturedItem = null;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .Callback<TodoItem, CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<TodoResponseDto>(It.IsAny<TodoItem>()))
            .Returns((TodoItem item) => MapToDto(item));

        // Act
        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New task");
        result.IsCompleted.Should().BeFalse();
        result.CreatedAt.Should().Be(_fixedUtcNow);
        result.Id.Should().NotBeEmpty();

        capturedItem.Should().NotBeNull();
        capturedItem!.Title.Should().Be("New task");
        capturedItem.IsCompleted.Should().BeFalse();
        capturedItem.CreatedAt.Should().Be(_fixedUtcNow);

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WhenItemExists_ReturnsUpdatedDto()
    {
        // Arrange
        var updateDto = new UpdateTodoDto("Updated title", true);

        var updatedItem = TodoItem.Create("Updated title", _fixedUtcNow.AddDays(-1));
        updatedItem.IsCompleted = true;
        var id = updatedItem.Id;

        _repositoryMock
            .Setup(r => r.UpdateAsync(id, updateDto.Title, updateDto.IsCompleted, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedItem);

        var expectedDto = MapToDto(updatedItem);
        _mapperMock
            .Setup(m => m.Map<TodoResponseDto>(updatedItem))
            .Returns(expectedDto);

        // Act
        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        _repositoryMock.Verify(
            r => r.UpdateAsync(id, "Updated title", true, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenItemNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateTodoDto("Title", false);

        _repositoryMock
            .Setup(r => r.UpdateAsync(id, updateDto.Title, updateDto.IsCompleted, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mapperMock.Verify(m => m.Map<TodoResponseDto>(It.IsAny<TodoItem>()), Times.Never);
    }

    #endregion

    #region SetCompletionStatusAsync

    [Fact]
    public async Task SetCompletionStatusAsync_MarkAsCompleted_ReturnsUpdatedDto()
    {
        // Arrange
        var item = CreateTodoItem("Task");
        item.IsCompleted = true;
        var dto = new UpdateTodoStatusDto(IsCompleted: true);
        var expectedDto = MapToDto(item);

        _repositoryMock
            .Setup(r => r.UpdateStatusAsync(item.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mapperMock
            .Setup(m => m.Map<TodoResponseDto>(item))
            .Returns(expectedDto);

        // Act
        var result = await _sut.UpdateStatusAsync(item.Id, dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();
        result.Should().BeEquivalentTo(expectedDto);
        _repositoryMock.Verify(
            r => r.UpdateStatusAsync(item.Id, true, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetCompletionStatusAsync_MarkAsIncomplete_ReturnsUpdatedDto()
    {
        // Arrange
        var item = CreateTodoItem("Done task", isCompleted: true);
        item.IsCompleted = false;
        var dto = new UpdateTodoStatusDto(IsCompleted: false);
        var expectedDto = MapToDto(item);

        _repositoryMock
            .Setup(r => r.UpdateStatusAsync(item.Id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mapperMock
            .Setup(m => m.Map<TodoResponseDto>(item))
            .Returns(expectedDto);

        // Act
        var result = await _sut.UpdateStatusAsync(item.Id, dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeFalse();
        _repositoryMock.Verify(
            r => r.UpdateStatusAsync(item.Id, false, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetCompletionStatusAsync_WhenItemNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateTodoStatusDto(IsCompleted: true);

        _repositoryMock
            .Setup(r => r.UpdateStatusAsync(id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _sut.UpdateStatusAsync(id, dto, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mapperMock.Verify(m => m.Map<TodoResponseDto>(It.IsAny<TodoItem>()), Times.Never);
    }

    #endregion


    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteAsync(id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemNotFound_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.DeleteAsync(id, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Helpers

    private static TodoItem CreateTodoItem(string title, bool isCompleted = false)
    {
        var item = TodoItem.Create(title, DateTime.UtcNow);
        item.IsCompleted = isCompleted;
        return item;
    }

    private static TodoResponseDto MapToDto(TodoItem item) =>
        new(item.Id, item.Title, item.IsCompleted, item.CreatedAt);

    private sealed class FakeTimeProvider(DateTime utcNow) : TimeProvider
    {
        private readonly DateTimeOffset _utcNow = new(utcNow);

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }

    #endregion
}
