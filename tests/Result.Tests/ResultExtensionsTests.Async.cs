using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class AsyncTests
    {
        public class MapAsyncTests
        {

            #region MapAsync Tests

            [Fact]
            public async Task MapAsync_WithAsyncMapper_OnSuccess_ReturnsSuccessWithMappedValue()
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
            public async Task MapAsync_WithAsyncMapper_OnFailure_PropagatesFailure()
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
            public async Task MapAsync_WithAsyncMapper_DoesNotExecuteMapperOnFailure()
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
            public async Task MapAsync_WithTaskResult_AndSyncMapper_ReturnsSuccess()
            {
                var resultTask = Task.FromResult(Result.Success(42));

                var mapped = await resultTask.MapAsync(x => x.ToString());

                Assert.True(mapped.IsSuccess);
                Assert.Equal("42", mapped.Value);
            }

            [Fact]
            public async Task MapAsync_WithTaskResult_AndAsyncMapper_ReturnsSuccess()
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
            public async Task MapAsync_WithTaskResult_ToNonGeneric_ReturnsSuccess()
            {
                var resultTask = Task.FromResult(Result.Success(42));

                var mapped = await resultTask.MapAsync();

                Assert.True(mapped.IsSuccess);
                Assert.IsType<Result>(mapped);
            }

            #endregion
        }

        public class BindAsyncTests
        {
            #region BindAsync Tests

            [Fact]
            public async Task BindAsync_WithAsyncBinder_OnSuccess_ReturnsBinderResult()
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
            public async Task BindAsync_WithAsyncBinder_OnFailure_PropagatesFailure()
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
            public async Task BindAsync_WithAsyncBinder_CanReturnFailureFromBinder()
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
            public async Task BindAsync_WithAsyncBinder_DoesNotExecuteBinderOnFailure()
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
            public async Task BindAsync_WithTaskResult_AndSyncBinder_ReturnsBinderResult()
            {
                var resultTask = Task.FromResult(Result.Success(42));

                var bound = await resultTask.BindAsync(x => Result.Success(x.ToString()));

                Assert.True(bound.IsSuccess);
                Assert.Equal("42", bound.Value);
            }

            [Fact]
            public async Task BindAsync_WithTaskResult_AndAsyncBinder_ReturnsBinderResult()
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
            public async Task BindAsync_ToNonGeneric_WithAsyncBinder_ReturnsBinderResult()
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
            public async Task BindAsync_FromNonGeneric_WithAsyncBinder_ReturnsBinderResult()
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
            public async Task BindAsync_NonGenericToNonGeneric_WithAsyncBinder_ReturnsBinderResult()
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
            public async Task MapAsync_BindAsync_CanChainAsyncOperations()
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
            public async Task MapAsync_BindAsync_ChainStopsAtFirstFailure()
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
            public async Task AsyncResultPipeline_RealWorldScenario()
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
}