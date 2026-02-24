using Xunit;

namespace Kubis1982.Result;

public class ResultExtensionsTests
{
    #region Map<T, TOut> Tests

    [Fact]
    public void Map_WithMapper_WhenResultIsSuccess_ReturnsSuccessWithMappedValue()
    {
        var result = Result.Success(42);

        var mapped = result.Map(x => x.ToString());

        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public void Map_WithMapper_WhenResultIsFailure_PropagatesFailure()
    {
        var error = Error.NotFound("not found");
        var result = Result.Failure<int>(error);

        var mapped = result.Map(x => x.ToString());

        Assert.False(mapped.IsSuccess);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Map_WithMapper_ExecutesMapperOnlyForSuccess()
    {
        var error = Error.Validation("validation error");
        var result = Result.Failure<int>(error);
        var mapperCalled = false;

        var mapped = result.Map(x =>
        {
            mapperCalled = true;
            return x.ToString();
        });

        Assert.False(mapperCalled);
        Assert.False(mapped.IsSuccess);
    }

    [Fact]
    public void Map_WithMapper_CanMapToComplexType()
    {
        var result = Result<int>.Success(42);

        var mapped = result.Map(x => new { Value = x, DoubleValue = x * 2 });

        Assert.True(mapped.IsSuccess);
        Assert.Equal(42, mapped.Value.Value);
        Assert.Equal(84, mapped.Value.DoubleValue);
    }

    #endregion

    #region Map<TSource> to Result Tests

    [Fact]
    public void Map_ToNonGeneric_WhenResultIsSuccess_ReturnsSuccess()
    {
        var result = Result.Success(42);

        var mapped = result.Map();

        Assert.True(mapped.IsSuccess);
    }

    [Fact]
    public void Map_ToNonGeneric_WhenResultIsFailure_PropagatesFailure()
    {
        var error = Error.Validation("E001", "custom error");
        var result = Result.Failure<string>(error);

        var mapped = result.Map();

        Assert.False(mapped.IsSuccess);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Map_ToNonGeneric_DiscardsValueType()
    {
        var result = Result.Success(999);

        var mapped = result.Map();

        Assert.True(mapped.IsSuccess);
        Assert.IsNotType<Result<int>>(mapped);
        Assert.IsType<Result>(mapped);
    }

    #endregion

    #region Map<TOut> from Result Tests

    [Fact]
    public void Map_FromNonGenericToGeneric_WhenResultIsFailure_PropagatesFailure()
    {
        var error = Error.Forbidden("access denied");
        var result = Result.Failure(error);

        var mapped = result.Map<string>();

        Assert.False(mapped.IsSuccess);
        Assert.Equal(error, mapped.Error);
    }

    #endregion

    #region Map Integration Tests

    [Fact]
    public void Map_CanChainMultipleMappings()
    {
        var result = Result.Success(10);

        var mapped = result
            .Map(x => x * 2)
            .Map(x => x + 5)
            .Map(x => x.ToString());

        Assert.True(mapped.IsSuccess);
        Assert.Equal("25", mapped.Value);
    }

    [Fact]
    public void Map_ChainStopsAtFirstFailure()
    {
        var error = Error.NotFound("not found");
        var result = Result.Failure<int>(error);

        var mapped = result
            .Map(x => x * 2)
            .Map(x => x + 5)
            .Map(x => x.ToString());

        Assert.False(mapped.IsSuccess);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Map_CanConvertGenericToNonGenericInChain()
    {
        var result = Result.Success(42);

        var mapped = result.Map();

        Assert.True(mapped.IsSuccess);
        Assert.IsType<Result>(mapped);
    }

    #endregion

    #region Bind<T, TOut> Tests

    [Fact]
    public void Bind_WithBinder_WhenResultIsSuccess_ReturnsBinderResult()
    {
        var result = Result.Success(42);

        var bound = result.Bind(x => Result.Success(x.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("42", bound.Value);
    }

    [Fact]
    public void Bind_WithBinder_WhenResultIsFailure_PropagatesFailure()
    {
        var error = Error.NotFound("not found");
        var result = Result.Failure<int>(error);

        var bound = result.Bind(x => Result.Success(x.ToString()));

        Assert.False(bound.IsSuccess);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void Bind_WithBinder_CanReturnFailureFromBinder()
    {
        var result = Result<int>.Success(42);
        var binderError = Error.Validation("validation failed");

        var bound = result.Bind(x => Result<string>.Failure(binderError));

        Assert.False(bound.IsSuccess);
        Assert.Equal(binderError, bound.Error);
    }

    [Fact]
    public void Bind_WithBinder_ExecutesBinderOnlyForSuccess()
    {
        var error = Error.Validation("validation error");
        var result = Result.Failure<int>(error);
        var binderCalled = false;

        var bound = result.Bind(x =>
        {
            binderCalled = true;
            return Result<string>.Success(x.ToString());
        });

        Assert.False(binderCalled);
        Assert.False(bound.IsSuccess);
    }

    [Fact]
    public void Bind_WithBinder_CanChainOperations()
    {
        var result = Result<int>.Success(10);

        var bound = result
            .Bind(x => Result<int>.Success(x * 2))
            .Bind(x => Result<int>.Success(x + 5))
            .Bind(x => Result<string>.Success(x.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("25", bound.Value);
    }

    [Fact]
    public void Bind_WithBinder_ChainStopsAtFirstFailure()
    {
        var result = Result<int>.Success(10);
        var error = Error.Validation("failed");

        var bound = result
            .Bind(x => Result.Success(x * 2))
            .Bind(x => Result.Failure<int>(error))
            .Bind(x => Result.Success<string>("should not execute"));

        Assert.False(bound.IsSuccess);
        Assert.Equal(error, bound.Error);
    }

    #endregion

    #region Bind<TSource> to Result Tests

    [Fact]
    public void Bind_ToNonGeneric_WhenResultIsSuccess_ReturnsBinderResult()
    {
        var result = Result.Success(42);

        var bound = result.Bind(x => Result.Success());

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public void Bind_ToNonGeneric_WhenResultIsFailure_PropagatesFailure()
    {
        var error = Error.Validation("E001", "custom error");
        var result = Result.Failure<string>(error);

        var bound = result.Bind(x => Result.Success());

        Assert.False(bound.IsSuccess);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void Bind_ToNonGeneric_CanReturnFailureFromBinder()
    {
        var result = Result<int>.Success(42);
        var binderError = Error.Unauthorized("unauthorized");

        var bound = result.Bind(x => Result.Failure(binderError));

        Assert.False(bound.IsSuccess);
        Assert.Equal(binderError, bound.Error);
    }

    [Fact]
    public void Bind_ToNonGeneric_ExecutesBinderOnlyForSuccess()
    {
        var error = Error.Validation("validation error");
        var result = Result.Failure<int>(error);
        var binderCalled = false;

        var bound = result.Bind(x =>
        {
            binderCalled = true;
            return Result.Success();
        });

        Assert.False(binderCalled);
        Assert.False(bound.IsSuccess);
    }

    #endregion

    #region Bind<TOut> from Result Tests

    [Fact]
    public void Bind_FromNonGenericToGeneric_WhenResultIsSuccess_ReturnsBinderResult()
    {
        var result = Result.Success();

        var bound = result.Bind(() => Result<string>.Success("hello"));

        Assert.True(bound.IsSuccess);
        Assert.Equal("hello", bound.Value);
    }

    [Fact]
    public void Bind_FromNonGenericToGeneric_WhenResultIsFailure_PropagatesFailure()
    {
        var error = Error.Forbidden("access denied");
        var result = Result.Failure(error);

        var bound = result.Bind(() => Result<string>.Success("hello"));

        Assert.False(bound.IsSuccess);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void Bind_FromNonGenericToGeneric_CanReturnFailureFromBinder()
    {
        var result = Result.Success();
        var binderError = Error.NotFound("not found");

        var bound = result.Bind(() => Result<int>.Failure(binderError));

        Assert.False(bound.IsSuccess);
        Assert.Equal(binderError, bound.Error);
    }

    [Fact]
    public void Bind_FromNonGenericToGeneric_ExecutesBinderOnlyForSuccess()
    {
        var error = Error.Validation("validation error");
        var result = Result.Failure(error);
        var binderCalled = false;

        var bound = result.Bind(() =>
        {
            binderCalled = true;
            return Result<string>.Success("value");
        });

        Assert.False(binderCalled);
        Assert.False(bound.IsSuccess);
    }

    #endregion

    #region Bind from Result to Result Tests

    [Fact]
    public void Bind_NonGenericToNonGeneric_WhenResultIsSuccess_ReturnsBinderResult()
    {
        var result = Result.Success();

        var bound = result.Bind(() => Result.Success());

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public void Bind_NonGenericToNonGeneric_WhenResultIsFailure_PropagatesFailure()
    {
        var error = Error.Validation("SERVER.ERROR", "server error");
        var result = Result.Failure(error);

        var bound = result.Bind(() => Result.Success());

        Assert.False(bound.IsSuccess);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void Bind_NonGenericToNonGeneric_CanReturnFailureFromBinder()
    {
        var s = Result.Success();
        var s1 = Result.Success(2);
        
        
        
        var result = Result.Success();
        var binderError = Error.Conflict("conflict");

        var bound = result.Bind(() => Result.Failure(binderError));

        Assert.False(bound.IsSuccess);
        Assert.Equal(binderError, bound.Error);
    }

    [Fact]
    public void Bind_NonGenericToNonGeneric_ExecutesBinderOnlyForSuccess()
    {
        var error = Error.Validation("validation error");
        var result = Result.Failure(error);
        var binderCalled = false;

        var bound = result.Bind(() =>
        {
            binderCalled = true;
            return Result.Success();
        });

        Assert.False(binderCalled);
        Assert.False(bound.IsSuccess);
    }

    [Fact]
    public void Bind_NonGenericToNonGeneric_CanChainMultipleOperations()
    {
        var result = Result.Success();

        var bound = result
            .Bind(() => Result.Success())
            .Bind(() => Result.Success())
            .Bind(() => Result.Success());

        Assert.True(bound.IsSuccess);
    }

    #endregion

    #region Bind Integration Tests

    [Fact]
    public void Bind_CanMixWithMapInChain()
    {
        var result = Result.Success(10);

        var final = result
            .Map(x => x * 2)
            .Bind(x => Result.Success(x + 5))
            .Map(x => x.ToString())
            .Bind(x => Result.Success($"Result: {x}"));

        Assert.True(final.IsSuccess);
        Assert.Equal("Result: 25", final.Value);
    }

    [Fact]
    public void Bind_ComplexChainWithMultipleTransformations()
    {
        var result = Result.Success("10");

        var final = result
            .Bind(x => int.TryParse(x, out var num) 
                ? Result.Success(num) 
                : Result.Failure<int>(Error.Validation("not a number")))
            .Bind(x => x > 0 
                ? Result.Success(x * 2) 
                : Result.Failure<int>(Error.Validation("not positive")))
            .Map(x => $"Value: {x}");

        Assert.True(final.IsSuccess);
        Assert.Equal("Value: 20", final.Value);
    }

    [Fact]
    public void Bind_ComplexChainStopsAtFirstFailureInBind()
    {
        var result = Result.Success("abc");

        var final = result
            .Bind(x => int.TryParse(x, out var num) 
                ? Result.Success(num) 
                : Result.Failure<int>(Error.Validation("not a number")))
            .Bind(x => Result.Success(x * 2))
            .Map(x => $"Value: {x}");

        Assert.False(final.IsSuccess);
        Assert.Equal(ErrorCodes.Validation, final.Error.Code);
    }

    #endregion
}
