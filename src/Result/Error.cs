namespace Kubis1982.Result;

/// <summary>
/// Represents a lightweight error descriptor used by <see cref="Result"/> to indicate failure.
/// Contains an error code and a human-readable description.
/// </summary>
public readonly record struct Error
{
    /// <summary>
    /// Type of the error.
    /// </summary>
    public ErrorType ErrorType { get; init; }

    /// <summary>
    /// Machine-friendly error code.
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Human-readable error description.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Private constructor to enforce the use of factory methods.
    /// </summary>
    private Error(ErrorType errorType, string code, string description)
    {
        ErrorType = errorType;
        Code = code;
        Description = description;
    }

    /// <summary>
    /// Creates a Not Found error with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Error NotFound(string description) => new(ErrorType.NotFound, ErrorCodes.NotFound, description);

    /// <summary>
    /// Creates a Conflict error with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Error Conflict(string description) => new(ErrorType.Conflict, ErrorCodes.Conflict, description);

    /// <summary>
    /// Creates a Forbidden error with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Error Forbidden(string description) => new(ErrorType.Forbidden, ErrorCodes.Forbidden, description);

    /// <summary>
    /// Creates an Unauthorized error with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Error Unauthorized(string description) => new(ErrorType.Unauthorized, ErrorCodes.Unauthorized, description);

    /// <summary>
    /// Creates a Validation error with the default error code.
    /// </summary>
    /// <param name="description">Human-readable error description.</param>
    public static Error Validation(string description) => new(ErrorType.Validation, ErrorCodes.Validation, description);

    /// <summary>
    /// Creates a Not Found error with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Error NotFound(string code, string description) => new(ErrorType.NotFound, code, description);

    /// <summary>
    /// Creates a Conflict error with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Error Conflict(string code, string description) => new(ErrorType.Conflict, code, description);

    /// <summary>
    /// Creates a Forbidden error with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Error Forbidden(string code, string description) => new(ErrorType.Forbidden, code, description);

    /// <summary>
    /// Creates an Unauthorized error with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Error Unauthorized(string code, string description) => new(ErrorType.Unauthorized, code, description);

    /// <summary>
    /// Creates a Validation error with a custom error code.
    /// </summary>
    /// <param name="code">Custom error code.</param>
    /// <param name="description">Human-readable error description.</param>
    public static Error Validation(string code, string description) => new(ErrorType.Validation, code, description);
}
