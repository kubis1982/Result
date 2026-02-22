namespace kubis1982.FluentResult;

public readonly record struct ResultError(string Code, string Description)
{
    public static ResultError NotFound(string description) => new(ResultErrorCodes.NotFound, description);
    public static ResultError Conflict(string description) => new(ResultErrorCodes.Conflict, description);
    public static ResultError Forbidden(string description) => new(ResultErrorCodes.Forbidden, description);
    public static ResultError Unauthorized(string description) => new(ResultErrorCodes.Unauthorized, description);
    public static ResultError Validation(string description) => new(ResultErrorCodes.Validation, description);
    public static ResultError Error(string code, string description) => new(code, description);
}

