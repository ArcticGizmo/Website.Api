namespace Website.Api.Common;

public record PagedData<T>(IReadOnlyList<T> Data, int Page, int PageSize, int? NextPage);