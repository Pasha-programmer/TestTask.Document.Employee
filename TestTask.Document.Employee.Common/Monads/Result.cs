namespace TestTask.Document.Employee.Common.Monads;

public readonly struct Result<V, E>
{
    public readonly V? Value;
    public readonly E? Error;
    public string? Details { get; }

    private readonly bool _isSuccess;

    public bool IsSuccess => _isSuccess;
    public bool IsFailed => !_isSuccess;

    private Result(V? value, E? error, bool isSuccess, string? detais = null)
    {
        Value = value;
        Error = error;
        _isSuccess = isSuccess;
        Details = detais;
    }

    public static Result<V, E> Success(V value, string details) => new(value, default(E), true, details);
    public static Result<V, E> Success(V value) => new(value, default(E), true);
    public static Result<V, E> Failed(E error) => new(default(V), error, false);
    public static Result<V, E> Failed(string details) => new(default(V), default(E), false, details);
    public static Result<V, E> Failed(string details, E error) => new(default(V), error, false, details);

    public static implicit operator Result<V, E>(V v) => new(v, default(E), true);
    public static implicit operator Result<V, E>(Result<E> r) => new(default(V), r.Value, false, r.Details);
    public static implicit operator Result<V, E>(Result r) => new(default(V), default(E), false, r.Details);
}

public readonly struct Result<T>
{
    public T Value { get; }
    public string? Details { get; }

    public bool IsSuccess => _isSuccess;
    public bool IsFailed => !_isSuccess;

    private readonly bool _isSuccess;

    private Result(bool isSucceeded, T value, string? details)
    {
        Value = value;
        _isSuccess = isSucceeded;
        Details = details;
    }

    public static Result<T> Success(T value, string? details = null) => new(true, value, details);
    public static Result<T> Failed(string details) => new(false, default, details);
    public static Result<T> Failed(T value, string? details = null) => new(false, value, details);

    public static implicit operator Result<T>(T value) => new(true, value, null);
    public static implicit operator Result<T>(Result r) => new(false, default(T), r.Details);
}

public readonly struct Result
{
    public string? Details { get; }

    public bool IsSuccess => _isSuccess;
    public bool IsFailed => !_isSuccess;

    private readonly bool _isSuccess;


    private Result(bool isSucceeded, string? details)
    {
        _isSuccess = isSucceeded;
        Details = details;
    }

    public static Result Success(string? details = null) => new(true, details);
    public static Result<T> Success<T>(T value, string? details = null) => Result<T>.Success(value, details);

    public static Result Failed(string details) => new(false, details);
    public static Result<T> Failed<T>(T value, string? details = null) => Result<T>.Failed(value, details);
}
