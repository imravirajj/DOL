namespace DOL.SharedKernel;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public List<string> Errors { get; }

    protected Result(bool isSuccess, string error, List<string>? errors = null)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Success result cannot have an error message.");
        if (!isSuccess && string.IsNullOrEmpty(error) && (errors == null || errors.Count == 0))
            throw new InvalidOperationException("Failure result must have an error message or list of errors.");

        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? (string.IsNullOrEmpty(error) ? new List<string>() : new List<string> { error });
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors) => new(false, errors.FirstOrDefault() ?? "Validation failed", errors);
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
    public static Result<T> Failure<T>(List<string> errors) => Result<T>.Failure(errors);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    protected Result(T? value, bool isSuccess, string error, List<string>? errors = null)
        : base(isSuccess, error, errors)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty);
    public static new Result<T> Failure(string error) => new(default, false, error);
    public static new Result<T> Failure(List<string> errors) => new(default, false, errors.FirstOrDefault() ?? "Validation failed", errors);
}
