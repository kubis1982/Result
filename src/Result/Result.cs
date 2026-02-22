namespace kubis1982.Result;

/// <summary>
/// Represents the outcome of an operation which can be either success or failure.
/// Contains an <see cref="IsSuccess"/> flag and an optional <see cref="Error"/> when failed.
/// </summary>
public partial class Result
{
    /// <summary>
    /// Indicates whether the result is a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// When the result represents a failure, contains the <see cref="ResultError"/> describing the failure; otherwise null.
    /// </summary>
    public ResultError? Error { get; }

    /// <summary>
    /// Protected constructor used by factory methods and derived types.
    /// </summary>
    /// <param name="isSuccess">Whether the result is success.</param>
    /// <param name="error">Optional error information for failure results.</param>
    protected Result(bool isSuccess, ResultError? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}

partial class Result
{
    #region Factory Methods

    /// <summary>
    /// Creates a non-generic successful result.
    /// </summary>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a generic successful result wrapping a value of <typeparamref name="T"/>.
    /// </summary>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    /// Creates a non-generic failure result with the provided <see cref="ResultError"/>.
    /// </summary>
    public static Result Failure(ResultError error) => new(false, error);

    /// <summary>
    /// Creates a generic failure result of type <typeparamref name="T"/> with the provided <see cref="ResultError"/>.
    /// </summary>
    public static Result<T> Failure<T>(ResultError error) => Result<T>.Failure(error);

    #endregion

    #region Operators

    /// <summary>
    /// Allows implicit conversion from <see cref="ResultError"/> to <see cref="Result"/>, producing a failure result.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(ResultError error) => Failure(error);

    #endregion
}
