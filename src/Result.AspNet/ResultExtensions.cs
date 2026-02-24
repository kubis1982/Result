using Microsoft.AspNetCore.Http;

namespace Kubis1982.Result;

/// <summary>
/// Extension methods for converting Result types to ASP.NET Core IResult responses.
/// These methods facilitate seamless integration between business logic results and HTTP responses.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to an ASP.NET Core IResult.
    /// Success results return HTTP 204 (No Content), while error results return appropriate HTTP status codes.
    /// </summary>
    /// <param name="result">The Result to convert.</param>
    /// <returns>
    /// For success: HTTP 204 No Content response.
    /// For errors: HTTP Problem Details response with appropriate status code based on error type.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
    public static IResult ToResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        // Success case: Return HTTP 204 NoContent
        if (result.IsSuccess) return TypedResults.NoContent();

        // Error case: Map error to appropriate HTTP status code and problem details
        var error = result.Error!.Value;
        var statusCode = GetStatusCode(error.Code);
        var title = GetTitle(error.Code);

        return Results.Problem(detail: error.Description, statusCode: statusCode, title: title, type: error.Code);
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to an ASP.NET Core IResult.
    /// Success results return HTTP 200 (OK) with the value, while error results return appropriate HTTP status codes.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <param name="result">The Result&lt;T&gt; to convert.</param>
    /// <returns>
    /// For success: HTTP 200 OK response with the result value as JSON.
    /// For errors: HTTP Problem Details response with appropriate status code based on error type.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
    public static IResult ToResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        // Success case: Return HTTP 200 OK with value
        if (result.IsSuccess) return TypedResults.Ok(result.Value);

        // Error case: Map error to appropriate HTTP status code and problem details
        var error = result.Error!.Value;
        var statusCode = GetStatusCode(error.Code);
        var title = GetTitle(error.Code);

        return Results.Problem(detail: error.Description, statusCode: statusCode, title: title, type: error.Code);
    }

    /// <summary>
    /// Maps error codes to appropriate HTTP status codes.
    /// Provides standard HTTP status code mapping for common business logic error types.
    /// </summary>
    /// <param name="code">The error code to map.</param>
    /// <returns>The corresponding HTTP status code.</returns>
    private static int GetStatusCode(string code) => code switch
    {
        ResultErrorCodes.NotFound => StatusCodes.Status404NotFound,         // 404
        ResultErrorCodes.Conflict => StatusCodes.Status409Conflict,         // 409
        ResultErrorCodes.Forbidden => StatusCodes.Status403Forbidden,       // 403
        ResultErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized, // 401
        ResultErrorCodes.Validation => StatusCodes.Status422UnprocessableEntity, // 422
        _ => StatusCodes.Status400BadRequest                                 // 400 (default)
    };

    /// <summary>
    /// Maps error codes to human-readable titles for HTTP Problem Details.
    /// Provides standardized error titles that correspond to HTTP status codes.
    /// </summary>
    /// <param name="code">The error code to map.</param>
    /// <returns>The corresponding human-readable title.</returns>
    private static string GetTitle(string code) => code switch
    {
        ResultErrorCodes.NotFound => "Not Found",
        ResultErrorCodes.Conflict => "Conflict",
        ResultErrorCodes.Forbidden => "Forbidden",
        ResultErrorCodes.Unauthorized => "Unauthorized",
        ResultErrorCodes.Validation => "Validation Error",
        _ => "Bad Request"
    };
}
