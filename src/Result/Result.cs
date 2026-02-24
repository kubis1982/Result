namespace Kubis1982.Result;

/// <summary>
/// Represents the outcome of an operation which can be either success or failure.
/// Contains an <see cref="IsSuccess"/> flag and an optional <see cref="Error"/> when failed.
/// </summary>
public partial class Result
{
    /// <summary>
    /// Indicates whether the result is a success.
    /// </summary>
    public bool IsSuccess => Error.ErrorType == ErrorType.None;

    /// <summary>
    /// Contains the <see cref="Kubis1982.Result.Error"/> describing the failure when <see cref="IsSuccess"/> is false; otherwise contains an error with <see cref="ErrorType.None"/>.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Protected constructor used by factory methods and derived types.
    /// </summary>
    private protected Result()
    {
    }

    /// <summary>
    /// Protected constructor used by factory methods and derived types.
    /// </summary>
    /// <param name="error">Error information for failure results.</param>
    private protected Result(Error error)
    {
        Error = error;
    }
}

partial class Result
{
    #region Factory Methods

    /// <summary>
    /// Creates a non-generic successful result.
    /// </summary>
    public static Result Success() => new();

    /// <summary>
    /// Creates a generic successful result wrapping a value of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap in the result.</param>
    public static Result<T> Success<T>(T value) => new(value);

    /// <summary>
    /// Creates a non-generic failure result with the provided <see cref="Kubis1982.Result.Error"/>.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    public static Result Failure(Error error) => new(error);

    /// <summary>
    /// Creates a generic failure result of type <typeparamref name="T"/> with the provided <see cref="Kubis1982.Result.Error"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="error">The error describing the failure.</param>
    public static Result<T> Failure<T>(Error error) => new(error);

    #endregion

    #region Operators

    /// <summary>
    /// Allows implicit conversion from <see cref="Kubis1982.Result.Error"/> to <see cref="Result"/>, producing a failure result.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) => Failure(error);

    #endregion
}
