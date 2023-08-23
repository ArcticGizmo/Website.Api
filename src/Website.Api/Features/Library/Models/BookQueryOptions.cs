namespace Website.Api.Features.Library.Models;

public class BookQueryOptions : ITextQueryOptions, IPagedQueryOptions, ISortable
{
    public string? SearchText { get; init; }

    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }

    public string? SortBy { get; init; } = "title";

    public bool? SortDecending { get; init; }
}