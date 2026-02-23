namespace Kubis1982.Result;

/// <summary>
/// Represents a lightweight error descriptor used by <see cref="Result"/> to indicate failure.
/// Contains an error code and a human-readable description.
/// </summary>
/// <param name="Code">Machine-friendly error code.</param>
/// <param name="Description">Human-readable error description.</param>
public readonly record struct ResultError(string Code, string Description)
{
    /// <summary>
    /// Creates a Not Found error with the default error code.
    /// </summary>
    public static ResultError NotFound(string description) => new(ResultErrorCodes.NotFound, description);

    /// <summary>
    /// Creates a Conflict error with the default error code.
    /// </summary>
    public static ResultError Conflict(string description) => new(ResultErrorCodes.Conflict, description);

    /// <summary>
    /// Creates a Forbidden error with the default error code.
    /// </summary>
    public static ResultError Forbidden(string description) => new(ResultErrorCodes.Forbidden, description);

    /// <summary>
    /// Creates an Unauthorized error with the default error code.
    /// </summary>
    public static ResultError Unauthorized(string description) => new(ResultErrorCodes.Unauthorized, description);

    /// <summary>
    /// Creates a Validation error with the default error code.
    /// </summary>
    public static ResultError Validation(string description) => new(ResultErrorCodes.Validation, description);

    /// <summary>
    /// Creates a custom error with the specified code and description.
    /// </summary>
    public static ResultError Error(string code, string description) => new(code, description);
}

