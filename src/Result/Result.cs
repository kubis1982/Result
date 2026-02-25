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

    #region Success

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

    #endregion

    #region Failure

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

    #region NotFound

    /// <summary>
    /// Creates a Not Found failure result with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Result NotFound(string description) => Error.NotFound(description);

    /// <summary>
    /// Creates a generic Not Found failure result with the default error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> NotFound<T>(string description) => Error.NotFound(description);

    /// <summary>
    /// Creates a Not Found failure result with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result NotFound(string code, string description) => Error.NotFound(code, description);

    /// <summary>
    /// Creates a generic Not Found failure result with a custom error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> NotFound<T>(string code, string description) => Error.NotFound(code, description);

    #endregion

    #region Conflict

    /// <summary>
    /// Creates a Conflict failure result with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Result Conflict(string description) => Error.Conflict(description);

    /// <summary>
    /// Creates a generic Conflict failure result with the default error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Conflict<T>(string description) => Error.Conflict(description);

    /// <summary>
    /// Creates a Conflict failure result with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result Conflict(string code, string description) => Error.Conflict(code, description);

    /// <summary>
    /// Creates a generic Conflict failure result with a custom error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Conflict<T>(string code, string description) => Error.Conflict(code, description);

    #endregion

    #region Forbidden

    /// <summary>
    /// Creates a Forbidden failure result with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Result Forbidden(string description) => Error.Forbidden(description);

    /// <summary>
    /// Creates a generic Forbidden failure result with the default error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Forbidden<T>(string description) => Error.Forbidden(description);

    /// <summary>
    /// Creates a Forbidden failure result with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result Forbidden(string code, string description) => Error.Forbidden(code, description);

    /// <summary>
    /// Creates a generic Forbidden failure result with a custom error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Forbidden<T>(string code, string description) => Error.Forbidden(code, description);

    #endregion

    #region Unauthorized

    /// <summary>
    /// Creates an Unauthorized failure result with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Result Unauthorized(string description) => Error.Unauthorized(description);

    /// <summary>
    /// Creates a generic Unauthorized failure result with the default error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Unauthorized<T>(string description) => Error.Unauthorized(description);

    /// <summary>
    /// Creates an Unauthorized failure result with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result Unauthorized(string code, string description) => Error.Unauthorized(code, description);

    /// <summary>
    /// Creates a generic Unauthorized failure result with a custom error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Unauthorized<T>(string code, string description) => Error.Unauthorized(code, description);

    #endregion

    #region Validation

    /// <summary>
    /// Creates a Validation failure result with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Result Validation(string description) => Error.Validation(description);

    /// <summary>
    /// Creates a generic Validation failure result with the default error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Validation<T>(string description) => Error.Validation(description);

    /// <summary>
    /// Creates a Validation failure result with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result Validation(string code, string description) => Error.Validation(code, description);

    /// <summary>
    /// Creates a generic Validation failure result with a custom error code.
    /// </summary>
    /// <typeparam name="T">The type of the value that would be returned on success.</typeparam>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Result<T> Validation<T>(string code, string description) => Error.Validation(code, description);

    #endregion

    #endregion

    #region Operators

    /// <summary>
    /// Allows implicit conversion from <see cref="Kubis1982.Result.Error"/> to <see cref="Result"/>, producing a failure result.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) => Failure(error);

    #endregion
}
