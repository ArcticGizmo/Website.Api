using MongoDB.Driver;
using Website.Api.Features.Recipes.Models;


namespace Website.Api.Common;

public static class QueryableExtensions
{
    public static IFindFluent<T, T> Paged<T>(this IFindFluent<T, T> query, IPagedQueryOptions? opts)
    {
        if (opts is null)
            return query;
        return query.Skip(opts.PageNumber * opts.PageSize).Limit(opts.PageSize);
    }

    public static IFindFluent<T, T> SortBy<T>(this IFindFluent<T, T> query, ISortable? opts)
    {
        if (opts is null || string.IsNullOrEmpty(opts.SortBy))
            return query;

        var fields = opts.SortBy.Split("|").Select(ToSortField).ToList();

        var sorter = Builders<T>.Sort.DirectionalSortBy(fields[0]);

        for (int i = 1; i < fields.Count; i++)
        {
            sorter = sorter.DirectionalSortBy(fields[i]);
        }

        return query.Sort(sorter);
    }

    private static SortField ToSortField(string text)
    {
        var parts = text.Split(":");

        if (parts.Length == 1)
        {
            return new SortField(text, true);
        }

        return new SortField(parts[0], parts[1] == "1");
    }

    private static SortDefinition<T> DirectionalSortBy<T>(this SortDefinitionBuilder<T> query, SortField field)
    {
        return field.Ascending ? query.Ascending(field.Field) : query.Descending(field.Field);
    }

    private static SortDefinition<T> DirectionalSortBy<T>(this SortDefinition<T> query, SortField field)
    {
        return field.Ascending ? query.Ascending(field.Field) : query.Descending(field.Field);
    }
}

internal record SortField(string Field, bool Ascending);


