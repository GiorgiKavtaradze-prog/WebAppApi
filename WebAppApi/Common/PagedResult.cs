namespace WebAppApi.Common;

public class PagedResult<T>
{
    public IEnumerable<T>? Data { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public string? Message { get; init; }
    public bool IsSuccess { get; init; }

    private PagedResult(
        IEnumerable<T>? data,
        int pageNumber,
        int pageSize,
        int totalCount,
        string? message,
        bool isSuccess)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = PageNumber < TotalPages;
        HasPreviousPage = PageNumber > 1;
        Message = message;
        IsSuccess = isSuccess;
    }

    public static PagedResult<T> Success(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalCount,
        string? message = "Data retrieved successfully")
    {
        return new PagedResult<T>(data, pageNumber, pageSize, totalCount, message, true);
    }

    public static PagedResult<T> Failure(string? message, int pageNumber = 1, int pageSize = 10)
    {
        return new PagedResult<T>(null, pageNumber, pageSize, 0, message, false);
    }
}