using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace Kubis1982.Result;

public class ResultExtensionsTests
{
    #region Success Tests

    [Fact]
    public void ToResult_WithSuccessResult_ReturnsNoContent()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var httpResult = result.ToResult();

        // Assert
        Assert.NotNull(httpResult);
        var noContent = Assert.IsType<NoContent>(httpResult);
        Assert.Equal(StatusCodes.Status204NoContent, noContent.StatusCode);
    }

    [Fact]
    public void ToResult_WithSuccessResultT_ReturnsOkWithValue()
    {
        // Arrange
        var expectedValue = 42;
        var result = Result.Success(expectedValue);

        // Act
        var httpResult = result.ToResult();

        // Assert
        Assert.NotNull(httpResult);
        var okResult = Assert.IsType<Ok<int>>(httpResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(expectedValue, okResult.Value);
    }

    [Fact]
    public void ToResult_WithSuccessResultT_AndComplexObject_ReturnsOkWithValue()
    {
        // Arrange
        var expectedUser = new TestUser { Id = 1, Name = "Test User" };
        var result = Result.Success(expectedUser);

        // Act
        var httpResult = result.ToResult();

        // Assert
        Assert.NotNull(httpResult);
        var okResult = Assert.IsType<Ok<TestUser>>(httpResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
        Assert.Equal(expectedUser.Id, okResult.Value.Id);
        Assert.Equal(expectedUser.Name, okResult.Value.Name);
    }

    #endregion

    #region Failure Tests - Non-Generic

    [Theory]
    [InlineData(ResultErrorCodes.NotFound, StatusCodes.Status404NotFound, "Not Found")]
    [InlineData(ResultErrorCodes.Conflict, StatusCodes.Status409Conflict, "Conflict")]
    [InlineData(ResultErrorCodes.Forbidden, StatusCodes.Status403Forbidden, "Forbidden")]
    [InlineData(ResultErrorCodes.Unauthorized, StatusCodes.Status401Unauthorized, "Unauthorized")]
    [InlineData(ResultErrorCodes.Validation, StatusCodes.Status422UnprocessableEntity, "Validation Error")]
    [InlineData("UNKNOWN_CODE", StatusCodes.Status400BadRequest, "Bad Request")]
    public void ToResult_WithFailureResult_ReturnsProblemDetailsWithCorrectStatusAndTitle(
        string errorCode,
        int expectedStatusCode,
        string expectedTitle)
    {
        // Arrange
        var errorDescription = "Test error description";
        var error = new ResultError(errorCode, errorDescription);
        var result = Result.Failure(error);

        // Act
        var httpResult = result.ToResult();

        // Assert
        Assert.NotNull(httpResult);
        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.NotNull(problemResult.ProblemDetails);
        Assert.Equal(expectedStatusCode, problemResult.StatusCode);
        Assert.Equal(expectedStatusCode, problemResult.ProblemDetails.Status);
        Assert.Equal(expectedTitle, problemResult.ProblemDetails.Title);
        Assert.Equal(errorDescription, problemResult.ProblemDetails.Detail);
        Assert.Equal(errorCode, problemResult.ProblemDetails.Type);
    }

    #endregion

    #region Failure Tests - Generic

    [Theory]
    [InlineData(ResultErrorCodes.NotFound, StatusCodes.Status404NotFound, "Not Found")]
    [InlineData(ResultErrorCodes.Conflict, StatusCodes.Status409Conflict, "Conflict")]
    [InlineData(ResultErrorCodes.Forbidden, StatusCodes.Status403Forbidden, "Forbidden")]
    [InlineData(ResultErrorCodes.Unauthorized, StatusCodes.Status401Unauthorized, "Unauthorized")]
    [InlineData(ResultErrorCodes.Validation, StatusCodes.Status422UnprocessableEntity, "Validation Error")]
    [InlineData("UNKNOWN_CODE", StatusCodes.Status400BadRequest, "Bad Request")]
    public void ToResult_WithFailureResultT_ReturnsProblemDetailsWithCorrectStatusAndTitle(
        string errorCode,
        int expectedStatusCode,
        string expectedTitle)
    {
        // Arrange
        var errorDescription = "Test error description for generic result";
        var error = new ResultError(errorCode, errorDescription);
        var result = Result.Failure<int>(error);

        // Act
        var httpResult = result.ToResult();

        // Assert
        Assert.NotNull(httpResult);
        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.NotNull(problemResult.ProblemDetails);
        Assert.Equal(expectedStatusCode, problemResult.StatusCode);
        Assert.Equal(expectedStatusCode, problemResult.ProblemDetails.Status);
        Assert.Equal(expectedTitle, problemResult.ProblemDetails.Title);
        Assert.Equal(errorDescription, problemResult.ProblemDetails.Detail);
        Assert.Equal(errorCode, problemResult.ProblemDetails.Type);
    }

    #endregion
}

public class TestUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
