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

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ImplicitOperator_FromError_ReturnsFailureResult()
    {
        var error = Error.Validation("bad");
        Result result = error;

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Success_WithValue_ReturnsSuccessWithValue()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
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

        Assert.True(result.IsFailure);
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

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void TryGetValue_OnSuccessResult_ReturnsTrueAndValue()
    {
        var result = Result.Success(42);

        var success = result.TryGetValue(out var value);

        Assert.True(success);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryGetValue_OnFailureResult_ReturnsFalseAndDefault()
    {
        var result = Result.NotFound<int>("not found");

        var success = result.TryGetValue(out var value);

        Assert.False(success);
        Assert.Equal(default, value);
    }

    [Fact]
    public void TryGetValue_OnSuccessWithString_ReturnsTrueAndValue()
    {
        var result = Result.Success("hello");

        var success = result.TryGetValue(out var value);

        Assert.True(success);
        Assert.Equal("hello", value);
    }

    [Fact]
    public void TryGetValue_OnFailureWithString_ReturnsFalseAndNull()
    {
        var result = Result.Validation<string>("validation error");

        var success = result.TryGetValue(out var value);

        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void TryGetValue_OnSuccessWithReferenceType_ReturnsTrueAndValue()
    {
        var user = new { Id = 1, Name = "John" };
        var result = Result.Success(user);

        var success = result.TryGetValue(out var value);

        Assert.True(success);
        Assert.Equal(user, value);
    }

    [Fact]
    public void TryGetValue_CanBeUsedInIfStatement()
    {
        var result = Result.Success(100);

        if (result.TryGetValue(out var value))
        {
            Assert.Equal(100, value);
        }
        else
        {
            Assert.Fail("Should not reach this branch");
        }
    }

    [Fact]
    public void Deconstruct_OnSuccessResult_ReturnsSuccessState()
    {
        var result = Result.Success();

        var (isSuccess, error) = result;

        Assert.True(isSuccess);
        Assert.Equal(ErrorType.None, error.ErrorType);
    }

    [Fact]
    public void Deconstruct_OnFailureResult_ReturnsFailureStateAndError()
    {
        var expectedError = Error.NotFound("not found");
        var result = Result.Failure(expectedError);

        var (isSuccess, error) = result;

        Assert.False(isSuccess);
        Assert.Equal(expectedError, error);
    }

    [Fact]
    public void Deconstruct_Generic_OnSuccessResult_ReturnsSuccessStateAndValue()
    {
        var result = Result.Success(42);

        var (isSuccess, value, error) = result;

        Assert.True(isSuccess);
        Assert.Equal(42, value);
        Assert.Equal(ErrorType.None, error.ErrorType);
    }

    [Fact]
    public void Deconstruct_Generic_OnFailureResult_ReturnsFailureStateAndError()
    {
        var expectedError = Error.Validation("validation error");
        var result = Result.Failure<int>(expectedError);

        var (isSuccess, value, error) = result;

        Assert.False(isSuccess);
        Assert.Equal(default, value);
        Assert.Equal(expectedError, error);
    }

    [Fact]
    public void Deconstruct_Generic_WithString_OnSuccess_ReturnsValue()
    {
        var result = Result.Success("hello");

        var (isSuccess, value, error) = result;

        Assert.True(isSuccess);
        Assert.Equal("hello", value);
        Assert.Equal(ErrorType.None, error.ErrorType);
    }

    [Fact]
    public void Deconstruct_Generic_WithString_OnFailure_ReturnsNull()
    {
        var expectedError = Error.Forbidden("forbidden");
        var result = Result.Failure<string>(expectedError);

        var (isSuccess, value, error) = result;

        Assert.False(isSuccess);
        Assert.Null(value);
        Assert.Equal(expectedError, error);
    }

    [Fact]
    public void Deconstruct_CanBeUsedInSwitch()
    {
        var result = Result.Success(100);

        var output = result switch
        {
            (true, var val, _) => $"Success: {val}",
            (false, _, var err) => $"Error: {err.Description}",
        };

        Assert.Equal("Success: 100", output);
    }

    [Fact]
    public void Deconstruct_FailureCase_CanBeUsedInSwitch()
    {
        var result = Result.Failure<int>(Error.NotFound("Item not found"));

        var output = result switch
        {
            (true, var val, _) => $"Success: {val}",
            (false, _, var err) => $"Error: {err.Description}",
        };

        Assert.Equal("Error: Item not found", output);
    }

    [Fact]
    public void Combine_StaticMethod_WithAllSuccessfulResults_ReturnsSuccess()
    {
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        var combined = Result.Combine(result1, result2, result3);

        Assert.True(combined.IsSuccess);
    }

    [Fact]
    public void Combine_StaticMethod_WithFailure_ReturnsFirstFailure()
    {
        var result1 = Result.Success();
        var error = Error.NotFound("not found");
        var result2 = Result.Failure(error);
        var result3 = Result.Success();

        var combined = Result.Combine(result1, result2, result3);

        Assert.True(combined.IsFailure);
        Assert.Equal(error, combined.Error);
    }

    [Fact]
    public void Combine_StaticMethod_Generic_WithAllSuccessfulResults_ReturnsSuccessWithValues()
    {
        var result1 = Result.Success(10);
        var result2 = Result.Success(20);
        var result3 = Result.Success(30);

        var combined = Result.Combine(result1, result2, result3);

        Assert.True(combined.IsSuccess);
        Assert.Equal([10, 20, 30], combined.Value);
    }

    [Fact]
    public void Combine_StaticMethod_Generic_WithFailure_ReturnsFirstFailure()
    {
        var result1 = Result.Success(10);
        var error = Error.Validation("validation error");
        var result2 = Result.Failure<int>(error);
        var result3 = Result.Success(30);

        var combined = Result.Combine(result1, result2, result3);

        Assert.True(combined.IsFailure);
        Assert.Equal(error, combined.Error);
    }
}
