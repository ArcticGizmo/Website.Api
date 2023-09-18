using Website.Api.Common;

namespace Website.Api.Features.Recipes.Models;

public class RecipeQueryOptions : ITextQueryOptions, IPagedQueryOptions, ISortable
{
    public string? SearchText { get; init; }

    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }

    public string? SortBy { get; init; } = "title";
}