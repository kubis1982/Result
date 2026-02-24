using Xunit;

namespace Kubis1982.Result;

public class ResultTests
{
    [Fact]
    public void Success_WhenCalled_ReturnsSuccessResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Failure_WithError_ReturnsFailureWithError()
    {
        var error = Error.Validation("E001", "failure");
        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ImplicitOperator_FromError_ReturnsFailureResult()
    {
        var error = Error.Validation("bad");
        Result result = error;

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Success_WithValue_ReturnsSuccessWithValue()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ExplicitOperator_OnSuccessResult_ReturnsValue()
    {
        var result = Result.Success(42);

        int value = (int)result;

        Assert.Equal(42, value);
    }

    [Fact]
    public void Failure_WithError_ReturnsFailureResult()
    {
        var error = Error.NotFound("not found");
        var result = Result.Failure<int>(error);

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Value_OnFailureResult_ThrowsResultException()
    {
        var error = Error.NotFound("not found");
        var result = Result.Failure<int>(error);

        var ex = Assert.Throws<ResultException>(() => _ = result.Value);

        Assert.Equal(error, ex.Error);
    }

    [Fact]
    public void ImplicitOperator_FromValue_ReturnsSuccessResult()
    {
        Result<int> result = 7;

        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Value);
    }

    [Fact]
    public void ImplicitOperatorGeneric_FromError_ReturnsFailureResult()
    {
        var error = Error.Forbidden("no");

        Result<int> result = error;

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }
}
