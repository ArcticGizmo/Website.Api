namespace Website.Api.Common;

public interface IPagedQueryOptions
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}

public interface ITextQueryOptions
{
    public string? SearchText { get; init; }
}

public interface ISortable
{
    public string? SortBy { get; init; }
    public bool? SortDecending { get; init; }
}
