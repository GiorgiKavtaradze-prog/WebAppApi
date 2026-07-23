namespace WebAppApi.Common;

public class Result<T>
{
    public T? Data { get; init; }
    public string? Message { get; init; }
    public bool IsSuccess { get; init; }

    private Result(T? data, string? message, bool isSuccess)
    {
        Data = data;
        Message = message;
        IsSuccess = isSuccess;
    }

    public static Result<T> Success(T? data, string? message = "Operation completed successfully")
    {
        return new Result<T>(data, message, true);
    }

    public static Result<T> Failure(string? message, T? data = default)
    {
        return new Result<T>(data, message, false);
    }
}

public class Result
{
    public string? Message { get; init; }
    public bool IsSuccess { get; init; }

    private Result(string? message, bool isSuccess)
    {
        Message = message;
        IsSuccess = isSuccess;
    }

    public static Result Success(string? message = "Operation completed successfully")
    {
        return new Result(message, true);
    }

    public static Result Failure(string? message)
    {
        return new Result(message, false);
    }
}