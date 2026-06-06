namespace Embe.C2C.Application.Abstractions;

public class ResultBase<T_FailureReason>
{
    protected ResultBase()
    {
        IsSuccess = true;
    }

    protected ResultBase(T_FailureReason reason, string message)
    {
        Reason = reason;
        IsSuccess = false;
        Message = message;
    }

    public bool IsSuccess { get; }
    public T_FailureReason? Reason { get; }
    public string? Message { get; }
    public static ResultBase<T_FailureReason> Success() => new();
    public static ResultBase<T_FailureReason> Failure(T_FailureReason reason, string message) => new(reason, message);
}

public class Result : ResultBase<FailureReason>
{
    protected Result() : base() { }
    protected Result(FailureReason reason, string message) : base(reason, message) { }

    new public static Result Success() => new();
    new public static Result Failure(FailureReason reason, string message) => new(reason, message);
}

public class TypedResult<T_FailureReason, T> : ResultBase<T_FailureReason>
{
    protected TypedResult(T value) : base()
    {
        Value = value;
    }

    protected TypedResult(T_FailureReason reason, string message) : base(reason, message)
    {
        Value = default;
    }

    public T? Value { get; }
    public static TypedResult<T_FailureReason, T> Success(T value) => new(value);
    new public static TypedResult<T_FailureReason, T> Failure(T_FailureReason reason, string message) => new(reason, message);
}

public class Result<T> : TypedResult<FailureReason, T>
{
    private Result(T value) : base(value)
    {

    }

    private Result(FailureReason reason, string message) : base(reason, message)
    {

    }

    new public static Result<T> Success(T value) => new(value);
    new public static Result<T> Failure(FailureReason reason, string message) => new(reason, message);
}

public enum FailureReason
{
    NotFound,
    Forbidden,
    DomainError,
    Unknown
}