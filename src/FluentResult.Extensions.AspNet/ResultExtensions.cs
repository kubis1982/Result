using Microsoft.AspNetCore.Http;

namespace kubis1982.FluentResult;

public static class ResultExtensions
{
    public static IResult ToResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess) return Results.Ok();

        var error = result.Error!.Value;
        var statusCode = GetStatusCode(error.Code);
        var title = GetTitle(error.Code);

        return Results.Problem(detail: error.Description, statusCode: statusCode, title: title, type: error.Code);
    }

    public static IResult ToResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Error!.Value;
        var statusCode = GetStatusCode(error.Code);
        var title = GetTitle(error.Code);

        return Results.Problem(detail: error.Description, statusCode: statusCode, title: title, type: error.Code);
    }

    private static int GetStatusCode(string code) => code switch
    {
        ResultErrorCodes.NotFound => StatusCodes.Status404NotFound,
        ResultErrorCodes.Conflict => StatusCodes.Status409Conflict,
        ResultErrorCodes.Forbidden => StatusCodes.Status403Forbidden,
        ResultErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
        ResultErrorCodes.Validation => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status400BadRequest
    };

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
