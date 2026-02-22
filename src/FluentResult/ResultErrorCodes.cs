namespace kubis1982.FluentResult;

/// <summary>
/// Common error code constants used when creating <see cref="ResultError"/> instances.
/// </summary>
public static class ResultErrorCodes
{
    /// <summary>Not found error code.</summary>
    public const string NotFound = "general.notfound";

    /// <summary>Conflict error code.</summary>
    public const string Conflict = "general.conflict";

    /// <summary>Forbidden error code.</summary>
    public const string Forbidden = "general.forbidden";

    /// <summary>Unauthorized error code.</summary>
    public const string Unauthorized = "general.unauthorized";

    /// <summary>Validation error code.</summary>
    public const string Validation = "general.validation";
}
