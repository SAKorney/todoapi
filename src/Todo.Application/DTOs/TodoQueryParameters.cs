namespace Todo.Application.DTOs;

public sealed record TodoQueryParameters(
    int Page = 1,
    int PageSize = 10,
    bool? IsCompleted = null,
    string? Search = null,
    string SortBy = "createdAt",
    string SortDir = "desc")
{
    private const int MaxPageSize = 100;
}

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
