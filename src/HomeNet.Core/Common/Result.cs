using HomeNet.Core.Common.Errors;

namespace HomeNet.Core.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }

    public Error? Error { get; }

    public T? Value { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) 
        => new Result<T>(true, value, null);

    public static Result<T> Failure(Error error) 
        => new Result<T>(false, default, error);

    public static implicit operator Task<Result<T>>(Result<T> result) 
        => Task.FromResult(result);
}

public static class Result
{
    public static Result<T> Success<T>(T value)
        => Result<T>.Success(value);
    
    public static Result<T> Failure<T>(Error error)
        => Result<T>.Failure(error);
}
