using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class MapAsyncTests
    {
        #region MapAsync Tests

        [Fact]
        public async Task Should_ReturnSuccessWithMappedValue_When_MapAsyncIsCalledWithAsyncMapperOnSuccess()
        {
            var result = Result.Success(42);

            var mapped = await result.MapAsync(async x =>
            {
                await Task.Delay(1);
                return x.ToString();
            });

            Assert.True(mapped.IsSuccess);
            Assert.Equal("42", mapped.Value);
        }

        [Fact]
        public async Task Should_PropagateFailure_When_MapAsyncIsCalledWithAsyncMapperOnFailure()
        {
            var error = Error.NotFound("not found");
            var result = Result.Failure<int>(error);

            var mapped = await result.MapAsync(async x =>
            {
                await Task.Delay(1);
                return x.ToString();
            });

            Assert.True(mapped.IsFailure);
            Assert.Equal(error, mapped.Error);
        }

        [Fact]
        public async Task Should_NotExecuteMapper_When_MapAsyncIsCalledOnFailure()
        {
            var error = Error.Validation("validation error");
            var result = Result.Failure<int>(error);
            var mapperExecuted = false;

            var mapped = await result.MapAsync(async x =>
            {
                await Task.Delay(1);
                mapperExecuted = true;
                return x.ToString();
            });

            Assert.False(mapperExecuted);
            Assert.True(mapped.IsFailure);
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_MapAsyncIsCalledOnTaskResultWithSyncMapper()
        {
            var resultTask = Task.FromResult(Result.Success(42));

            var mapped = await resultTask.MapAsync(x => x.ToString());

            Assert.True(mapped.IsSuccess);
            Assert.Equal("42", mapped.Value);
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_MapAsyncIsCalledOnTaskResultWithAsyncMapper()
        {
            var resultTask = Task.FromResult(Result.Success(42));

            var mapped = await resultTask.MapAsync(async x =>
            {
                await Task.Delay(1);
                return x.ToString();
            });

            Assert.True(mapped.IsSuccess);
            Assert.Equal("42", mapped.Value);
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_MapAsyncIsCalledOnTaskResultToNonGeneric()
        {
            var resultTask = Task.FromResult(Result.Success(42));

            var mapped = await resultTask.MapAsync();

            Assert.True(mapped.IsSuccess);
            Assert.IsType<Result>(mapped);
        }

        #endregion
    }
}