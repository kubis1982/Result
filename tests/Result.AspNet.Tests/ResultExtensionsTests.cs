using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.ObjectPool;
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

    public static TheoryData<Error, int, string> GetErrorTestData()
    {
        return new TheoryData<Error, int, string>
        {
            { Error.NotFound("Not Found"), StatusCodes.Status404NotFound, "Not Found" },
            { Error.Conflict("Conflict"), StatusCodes.Status409Conflict, "Conflict" },
            { Error.Forbidden("Forbidden"), StatusCodes.Status403Forbidden, "Forbidden" },
            { Error.Unauthorized("Unauthorized"), StatusCodes.Status401Unauthorized, "Unauthorized" },
            { Error.Validation("Validation Error"), StatusCodes.Status422UnprocessableEntity, "Validation Error" },
            { Error.Unexpected("Unexpected Error"), StatusCodes.Status500InternalServerError, "Internal Server Error" }
        };
    }


    [Theory]
    [MemberData(nameof(GetErrorTestData), DisableDiscoveryEnumeration = true)]
    public void ToResult_WithFailureResult_ReturnsProblemDetailsWithCorrectStatusAndTitle(
        Error error,
        int expectedStatusCode,
        string expectedTitle)
    {
        // Arrange
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
        Assert.Equal(error.Description, problemResult.ProblemDetails.Detail);
        Assert.Equal(error.Code, problemResult.ProblemDetails.Type);
    }

    #endregion

    #region Failure Tests - Generic

    [Theory]
    [MemberData(nameof(GetErrorTestData), DisableDiscoveryEnumeration = true)]
    public void ToResult_WithFailureResultT_ReturnsProblemDetailsWithCorrectStatusAndTitle(
        Error error,
        int expectedStatusCode,
        string expectedTitle)
    {
        // Arrange
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
        Assert.Equal(error.Description, problemResult.ProblemDetails.Detail);
        Assert.Equal(error.Code, problemResult.ProblemDetails.Type);
    }

    #endregion
}

public class TestUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
