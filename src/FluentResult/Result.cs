namespace kubis1982.FluentResult;

public partial class Result
{
    public bool IsSuccess { get; }
    public ResultError? Error { get; }

    protected Result(bool isSuccess, ResultError? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}

partial class Result
{
    #region Factory Methods

    public static Result Success() => new(true);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result Failure(ResultError error) => new(false, error);

    public static Result<T> Failure<T>(ResultError error) => Result<T>.Failure(error);

    #endregion

    #region Operators

    public static implicit operator Result(ResultError error) => Failure(error);

    #endregion
}
