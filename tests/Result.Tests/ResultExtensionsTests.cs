using Xunit;

namespace Kubis1982.Result;

public class ResultExtensionsTests
{
    #region Map<T, TOut> Tests

    [Fact]
    public void Map_WithMapper_WhenResultIsSuccess_ReturnsSuccessWithMappedValue()
    {
        var result = Result<int>.Success(42);

        var mapped = result.Map(x => x.ToString());

        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public void Map_WithMapper_WhenResultIsFailure_PropagatesFailure()
    {
        var error = ResultError.NotFound("not found");
        var result = Result<int>.Failure(error);

        var mapped = result.Map(x => x.ToString());

        Assert.False(mapped.IsSuccess);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Map_WithMapper_ExecutesMapperOnlyForSuccess()
    {
        var error = ResultError.Validation("validation error");
        var result = Result<int>.Failure(error);
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
        var result = Result<int>.Success(42);

        var mapped = result.Map();

        Assert.True(mapped.IsSuccess);
        Assert.Null(mapped.Error);
    }

    [Fact]
    public void Map_ToNonGeneric_WhenResultIsFailure_PropagatesFailure()
    {
        var error = ResultError.Custom("E001", "custom error");
        var result = Result<string>.Failure(error);

        var mapped = result.Map();

        Assert.False(mapped.IsSuccess);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Map_ToNonGeneric_DiscardsValueType()
    {
        var result = Result<int>.Success(999);

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
        var error = ResultError.Forbidden("access denied");
        var result = Result.Failure(error);

        var mapped = result.Map<string>();

        Assert.False(mapped.IsSuccess);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Map_FromNonGenericToGeneric_WhenResultIsSuccess_ThrowsInvalidOperationException()
    {
        var result = Result.Success();

        var ex = Assert.Throws<InvalidOperationException>(() => result.Map<string>());

        Assert.Contains("Cannot map success result without mapping function", ex.Message);
    }

    [Fact]
    public void Map_FromNonGenericToGeneric_ExceptionMessageSuggestsCorrectOverload()
    {
        var result = Result.Success();

        var ex = Assert.Throws<InvalidOperationException>(() => result.Map<int>());

        Assert.Contains("Use Map<TOut>(Func<T, TOut> mapper) instead", ex.Message);
    }

    #endregion

    #region Map Integration Tests

    [Fact]
    public void Map_CanChainMultipleMappings()
    {
        var result = Result<int>.Success(10);

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
        var error = ResultError.NotFound("not found");
        var result = Result<int>.Failure(error);

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
        var result = Result<int>.Success(42);

        var mapped = result.Map();

        Assert.True(mapped.IsSuccess);
        Assert.IsType<Result>(mapped);
    }

    #endregion
}
