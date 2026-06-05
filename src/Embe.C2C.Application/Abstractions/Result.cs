namespace Embe.C2C.Application.Abstractions;

public class Result
{
    protected Result()
    {
        IsSuccess = true;
    }

    protected Result(FailureReason reason, string message)
    {
        Reason = reason;
        IsSuccess = false;
        Message = message;
    }

    public bool IsSuccess { get; }

    public FailureReason? Reason {get;}
    public string? Message { get; }

    public static Result Success() => new();
    public static Result Failure(FailureReason reason, string message) => new(reason, message);
}

public class Result<T> : Result
{
    private Result(T value) : base()
    {
        Value = value;
    }

    private Result(FailureReason reason, string message) : base(reason, message)
    {
        Value = default;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(value);
    new public static Result<T> Failure(FailureReason reason, string message) => new(reason, message);
}

public enum FailureReason
{
    NotFound,
    Forbidden,
    DomainError,
    Unknown
}