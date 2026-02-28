using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class BindAsyncTests
    {
        #region BindAsync Tests

        [Fact]
        public async Task Should_ReturnBinderResult_When_BindAsyncIsCalledWithAsyncBinderOnSuccess()
        {
            var result = Result.Success(42);

            var bound = await result.BindAsync(async x =>
            {
                await Task.Delay(1);
                return Result.Success(x.ToString());
            });

            Assert.True(bound.IsSuccess);
            Assert.Equal("42", bound.Value);
        }

        [Fact]
        public async Task Should_PropagateFailure_When_BindAsyncIsCalledWithAsyncBinderOnFailure()
        {
            var error = Error.NotFound("not found");
            var result = Result.Failure<int>(error);

            var bound = await result.BindAsync(async x =>
            {
                await Task.Delay(1);
                return Result.Success(x.ToString());
            });

            Assert.True(bound.IsFailure);
            Assert.Equal(error, bound.Error);
        }

        [Fact]
        public async Task Should_ReturnFailureFromBinder_When_BindAsyncIsUsed()
        {
            var result = Result.Success(42);
            var binderError = Error.Validation("validation failed");

            var bound = await result.BindAsync(async x =>
            {
                await Task.Delay(1);
                return Result.Failure<string>(binderError);
            });

            Assert.True(bound.IsFailure);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public async Task Should_NotExecuteBinder_When_BindAsyncIsCalledOnFailure()
        {
            var error = Error.Validation("validation error");
            var result = Result.Failure<int>(error);
            var binderExecuted = false;

            var bound = await result.BindAsync(async x =>
            {
                await Task.Delay(1);
                binderExecuted = true;
                return Result.Success(x.ToString());
            });

            Assert.False(binderExecuted);
            Assert.True(bound.IsFailure);
        }

        [Fact]
        public async Task Should_ReturnBinderResult_When_BindAsyncIsCalledOnTaskResultWithSyncBinder()
        {
            var resultTask = Task.FromResult(Result.Success(42));

            var bound = await resultTask.BindAsync(x => Result.Success(x.ToString()));

            Assert.True(bound.IsSuccess);
            Assert.Equal("42", bound.Value);
        }

        [Fact]
        public async Task Should_ReturnBinderResult_When_BindAsyncIsCalledOnTaskResultWithAsyncBinder()
        {
            var resultTask = Task.FromResult(Result.Success(42));

            var bound = await resultTask.BindAsync(async x =>
            {
                await Task.Delay(1);
                return Result.Success(x.ToString());
            });

            Assert.True(bound.IsSuccess);
            Assert.Equal("42", bound.Value);
        }

        [Fact]
        public async Task Should_ReturnBinderResult_When_BindAsyncToNonGenericIsUsed()
        {
            var result = Result.Success(42);

            var bound = await result.BindAsync(async x =>
            {
                await Task.Delay(1);
                return Result.Success();
            });

            Assert.True(bound.IsSuccess);
        }

        [Fact]
        public async Task Should_ReturnBinderResult_When_BindAsyncFromNonGenericIsUsed()
        {
            var result = Result.Success();

            var bound = await result.BindAsync(async () =>
            {
                await Task.Delay(1);
                return Result.Success("hello");
            });

            Assert.True(bound.IsSuccess);
            Assert.Equal("hello", bound.Value);
        }

        [Fact]
        public async Task Should_ReturnBinderResult_When_BindAsyncNonGenericToNonGenericIsUsed()
        {
            var result = Result.Success();

            var bound = await result.BindAsync(async () =>
            {
                await Task.Delay(1);
                return Result.Success();
            });

            Assert.True(bound.IsSuccess);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Should_ChainAsyncOperations_When_MapAsyncAndBindAsyncAreUsed()
        {
            var result = Result.Success(10);

            var final = await result
                .MapAsync(async x =>
                {
                    await Task.Delay(1);
                    return x * 2;
                })
                .BindAsync(async x =>
                {
                    await Task.Delay(1);
                    return Result.Success(x + 5);
                })
                .MapAsync(async x =>
                {
                    await Task.Delay(1);
                    return x.ToString();
                });

            Assert.True(final.IsSuccess);
            Assert.Equal("25", final.Value);
        }

        [Fact]
        public async Task Should_StopAtFirstFailure_When_ChainingMapAsyncAndBindAsync()
        {
            var result = Result.Success(10);
            var error = Error.Validation("failed");

            var final = await result
                .MapAsync(async x =>
                {
                    await Task.Delay(1);
                    return x * 2;
                })
                .BindAsync(async x =>
                {
                    await Task.Delay(1);
                    return Result.Failure<int>(error);
                })
                .MapAsync(async x =>
                {
                    await Task.Delay(1);
                    return x.ToString();
                });

            Assert.True(final.IsFailure);
            Assert.Equal(error, final.Error);
        }

        [Fact]
        public async Task Should_HandleRealWorldScenario_When_UsingAsyncResultPipeline()
        {
            var userId = 123;

            // Simulate async operations
            static async Task<Result<string>> GetUserEmailAsync(int id)
            {
                await Task.Delay(1);
                return id > 0 ? Result.Success($"user{id}@example.com") : Result.NotFound<string>("User not found");
            }

            static async Task<Result<bool>> SendEmailAsync(string email)
            {
                await Task.Delay(1);
                return email.Contains("@") ? Result.Success(true) : Result.Validation<bool>("Invalid email");
            }

            var result = await GetUserEmailAsync(userId)
                .BindAsync(async email =>
                {
                    await Task.Delay(1);
                    return await SendEmailAsync(email);
                })
                .MapAsync(async success =>
                {
                    await Task.Delay(1);
                    return success ? "Email sent successfully" : "Email sending failed";
                });

            Assert.True(result.IsSuccess);
            Assert.Equal("Email sent successfully", result.Value);
        }

        #endregion
    }
}