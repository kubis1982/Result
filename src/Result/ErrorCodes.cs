namespace Kubis1982.Result;

/// <summary>
/// Common error code constants used when creating <see cref="Error"/> instances.
/// </summary>
public static class ErrorCodes
{
    /// <summary>Not found error code.</summary>
    public const string NotFound = "GENERAL.NOTFOUND";

    /// <summary>Conflict error code.</summary>
    public const string Conflict = "GENERAL.CONFLICT";

    /// <summary>Forbidden error code.</summary>
    public const string Forbidden = "GENERAL.FORBIDDEN";

    /// <summary>Unauthorized error code.</summary>
    public const string Unauthorized = "GENERAL.UNAUTHORIZED";

    /// <summary>Validation error code.</summary>
    public const string Validation = "GENERAL.VALIDATION";

    /// <summary>Unexpected error code.</summary>
    public const string Unexpected = "GENERAL.UNEXPECTED";
}
