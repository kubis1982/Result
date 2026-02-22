using Xunit;

namespace kubis1982.Result;

public class ResultTests
{
    [Fact]
    public void Success_WhenCalled_ReturnsSuccessResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_WithError_ReturnsFailureWithError()
    {
        var error = ResultError.Error("E001", "failure");
        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ImplicitOperator_FromError_ReturnsFailureResult()
    {
        var error = ResultError.Validation("bad");
        Result result = error;

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Success_WithValue_ReturnsSuccessWithValue()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Equal(42, result.ValueOrDefault);
    }

    [Fact]
    public void ExplicitOperator_OnSuccessResult_ReturnsValue()
    {
        var result = Result<int>.Success(42);

        int value = (int)result;

        Assert.Equal(42, value);
    }

    [Fact]
    public void Failure_WithError_ReturnsFailureResult()
    {
        var error = ResultError.NotFound("not found");
        var result = Result<int>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Value_OnFailureResult_ThrowsResultException()
    {
        var error = ResultError.NotFound("not found");
        var result = Result<int>.Failure(error);

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
        var error = ResultError.Forbidden("no");

        Result<int> result = error;

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ValueOrDefault_OnSuccessResult_ReturnsValue()
    {
        var result = Result<string>.Success("abc");

        var value = result.ValueOrDefault;

        Assert.Equal("abc", value);
    }

    [Fact]
    public void ValueOrDefault_OnFailureResult_ReturnsDefault()
    {
        var error = ResultError.Error("E2", "failed");
        var result = Result<string>.Failure(error);

        var value = result.ValueOrDefault;

        Assert.Null(value);
    }
}
