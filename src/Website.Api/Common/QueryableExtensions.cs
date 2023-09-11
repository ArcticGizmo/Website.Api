using MongoDB.Driver;


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

        if (opts.SortDecending == true)
        {
            var sorter = Builders<T>.Sort.Descending(opts.SortBy);
            return query.Sort(sorter);
        }
        else
        {
            var sorter = Builders<T>.Sort.Ascending(opts.SortBy);
            return query.Sort(sorter);
        }
    }
}


