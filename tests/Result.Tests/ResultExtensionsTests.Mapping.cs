using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class MappingTests
    {

        #region Map<T, TOut> Tests

        [Fact]
        public void Should_ReturnSuccessWithMappedValue_When_ResultIsSuccessAndMapperIsProvided()
        {
            var result = Result.Success(42);

            var mapped = result.Map(x => x.ToString());

            Assert.True(mapped.IsSuccess);
            Assert.Equal("42", mapped.Value);
        }

        [Fact]
        public void Should_PropagateFailure_When_ResultIsFailureAndMapperIsProvided()
        {
            var error = Error.NotFound("not found");
            var result = Result.Failure<int>(error);

            var mapped = result.Map(x => x.ToString());

            Assert.True(mapped.IsFailure);
            Assert.Equal(error, mapped.Error);
        }

        [Fact]
        public void Should_ExecuteMapperOnlyForSuccess_When_ResultIsFailure()
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
            Assert.True(mapped.IsFailure);
        }

        [Fact]
        public void Should_MapToComplexType_When_MapperIsProvided()
        {
            var result = Result.Success(42);

            var mapped = result.Map(x => new { Value = x, DoubleValue = x * 2 });

            Assert.True(mapped.IsSuccess);
            Assert.Equal(42, mapped.Value.Value);
            Assert.Equal(84, mapped.Value.DoubleValue);
        }

        #endregion

        #region Map<TSource> to Result Tests

        [Fact]
        public void Should_ReturnSuccess_When_MappingToNonGenericFromSuccess()
        {
            var result = Result.Success(42);

            var mapped = result.Map();

            Assert.True(mapped.IsSuccess);
        }

        [Fact]
        public void Should_PropagateFailure_When_MappingToNonGenericFromFailure()
        {
            var error = Error.Validation("E001", "custom error");
            var result = Result.Failure<string>(error);

            var mapped = result.Map();

            Assert.True(mapped.IsFailure);
            Assert.Equal(error, mapped.Error);
        }

        [Fact]
        public void Should_DiscardValueType_When_MappingToNonGeneric()
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
        public void Should_PropagateFailure_When_MappingFromNonGenericToGenericWithFailure()
        {
            var error = Error.Forbidden("access denied");
            var result = Result.Failure(error);

            var mapped = result.Map<string>();

            Assert.True(mapped.IsFailure);
            Assert.Equal(error, mapped.Error);
        }

        #endregion

        #region Map Integration Tests

        [Fact]
        public void Should_ChainMultipleMappings_When_AllResultsAreSuccess()
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
        public void Should_StopAtFirstFailure_When_ChainingMappings()
        {
            var error = Error.NotFound("not found");
            var result = Result.Failure<int>(error);

            var mapped = result
                .Map(x => x * 2)
                .Map(x => x + 5)
                .Map(x => x.ToString());

            Assert.True(mapped.IsFailure);
            Assert.Equal(error, mapped.Error);
        }

        [Fact]
        public void Should_ConvertGenericToNonGeneric_When_MappingInChain()
        {
            var result = Result.Success(42);

            var mapped = result.Map();

            Assert.True(mapped.IsSuccess);
            Assert.IsType<Result>(mapped);
        }

        #endregion
    }
}
