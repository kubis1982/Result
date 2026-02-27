using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class MatchAsyncTests
    {
        #region MatchAsync Tests - Non-Generic Result with Async Functions

        [Fact]
        public async Task Should_CallOnSuccessFunction_When_MatchAsyncIsCalledOnNonGenericSuccess()
        {
            var result = Result.Success();

            var output = await result.MatchAsync(
                onSuccess: async () =>
                {
                    await Task.Delay(1);
                    return "Success";
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    return $"Error: {error.Description}";
                }
            );

            Assert.Equal("Success", output);
        }

        [Fact]
        public async Task Should_CallOnFailureFunction_When_MatchAsyncIsCalledOnNonGenericFailure()
        {
            var error = Error.NotFound("Resource not found");
            var result = Result.Failure(error);

            var output = await result.MatchAsync(
                onSuccess: async () =>
                {
                    await Task.Delay(1);
                    return "Success";
                },
                onFailure: async err =>
                {
                    await Task.Delay(1);
                    return $"Error: {err.Description}";
                }
            );

            Assert.Equal("Error: Resource not found", output);
        }

        [Fact]
        public async Task Should_NotCallOnFailure_When_NonGenericMatchAsyncOnSuccess()
        {
            var result = Result.Success();
            var onFailureCalled = false;

            await result.MatchAsync(
                onSuccess: async () =>
                {
                    await Task.Delay(1);
                    return "Success";
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    onFailureCalled = true;
                    return "Failure";
                }
            );

            Assert.False(onFailureCalled);
        }

        [Fact]
        public async Task Should_NotCallOnSuccess_When_NonGenericMatchAsyncOnFailure()
        {
            var result = Result.Failure(Error.Validation("Validation error"));
            var onSuccessCalled = false;

            await result.MatchAsync(
                onSuccess: async () =>
                {
                    await Task.Delay(1);
                    onSuccessCalled = true;
                    return "Success";
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    return "Failure";
                }
            );

            Assert.False(onSuccessCalled);
        }

        #endregion

        #region MatchAsync Tests - Task<Result> with Sync Functions

        [Fact]
        public async Task Should_CallOnSuccessFunction_When_MatchAsyncIsCalledOnTaskResult()
        {
            var resultTask = Task.FromResult(Result.Success());

            var output = await resultTask.MatchAsync(
                onSuccess: () => "Success",
                onFailure: error => $"Error: {error.Description}"
            );

            Assert.Equal("Success", output);
        }

        [Fact]
        public async Task Should_CallOnFailureFunction_When_MatchAsyncIsCalledOnTaskResultWithFailure()
        {
            var error = Error.NotFound("Resource not found");
            var resultTask = Task.FromResult(Result.Failure(error));

            var output = await resultTask.MatchAsync(
                onSuccess: () => "Success",
                onFailure: err => $"Error: {err.Description}"
            );

            Assert.Equal("Error: Resource not found", output);
        }

        #endregion

        #region MatchAsync Tests - Task<Result> with Async Functions

        [Fact]
        public async Task Should_CallOnSuccessFunction_When_MatchAsyncIsCalledOnTaskResultWithAsyncFunctions()
        {
            var resultTask = Task.FromResult(Result.Success());

            var output = await resultTask.MatchAsync(
                onSuccess: async () =>
                {
                    await Task.Delay(1);
                    return "Success";
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    return $"Error: {error.Description}";
                }
            );

            Assert.Equal("Success", output);
        }

        [Fact]
        public async Task Should_CallOnFailureFunction_When_MatchAsyncIsCalledOnTaskResultWithAsyncFunctionsAndFailure()
        {
            var error = Error.Conflict("Conflict occurred");
            var resultTask = Task.FromResult(Result.Failure(error));

            var output = await resultTask.MatchAsync(
                onSuccess: async () =>
                {
                    await Task.Delay(1);
                    return "Success";
                },
                onFailure: async err =>
                {
                    await Task.Delay(1);
                    return $"Error: {err.Description}";
                }
            );

            Assert.Equal("Error: Conflict occurred", output);
        }

        #endregion

        #region MatchAsync Tests - Generic Result with Async Functions

        [Fact]
        public async Task Should_CallOnSuccessFunctionWithValue_When_MatchAsyncIsCalledOnGenericSuccess()
        {
            var result = Result.Success(42);

            var output = await result.MatchAsync(
                onSuccess: async value =>
                {
                    await Task.Delay(1);
                    return $"Value: {value}";
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    return $"Error: {error.Description}";
                }
            );

            Assert.Equal("Value: 42", output);
        }

        [Fact]
        public async Task Should_CallOnFailureFunctionWithError_When_MatchAsyncIsCalledOnGenericFailure()
        {
            var error = Error.NotFound("User not found");
            var result = Result.Failure<int>(error);

            var output = await result.MatchAsync(
                onSuccess: async value =>
                {
                    await Task.Delay(1);
                    return $"Value: {value}";
                },
                onFailure: async err =>
                {
                    await Task.Delay(1);
                    return $"Error: {err.Description}";
                }
            );

            Assert.Equal("Error: User not found", output);
        }

        [Fact]
        public async Task Should_NotCallOnFailure_When_GenericMatchAsyncOnSuccess()
        {
            var result = Result.Success(100);
            var onFailureCalled = false;

            await result.MatchAsync(
                onSuccess: async value =>
                {
                    await Task.Delay(1);
                    return value.ToString();
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    onFailureCalled = true;
                    return "Failure";
                }
            );

            Assert.False(onFailureCalled);
        }

        [Fact]
        public async Task Should_NotCallOnSuccess_When_GenericMatchAsyncOnFailure()
        {
            var result = Result.Failure<string>(Error.Unauthorized("Unauthorized"));
            var onSuccessCalled = false;

            await result.MatchAsync(
                onSuccess: async value =>
                {
                    await Task.Delay(1);
                    onSuccessCalled = true;
                    return value;
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    return "Failure";
                }
            );

            Assert.False(onSuccessCalled);
        }

        #endregion

        #region MatchAsync Tests - Task<Result<T>> with Sync Functions

        [Fact]
        public async Task Should_CallOnSuccessFunctionWithValue_When_MatchAsyncIsCalledOnGenericTaskResult()
        {
            var resultTask = Task.FromResult(Result.Success(42));

            var output = await resultTask.MatchAsync(
                onSuccess: value => $"Value: {value}",
                onFailure: error => $"Error: {error.Description}"
            );

            Assert.Equal("Value: 42", output);
        }

        [Fact]
        public async Task Should_CallOnFailureFunctionWithError_When_MatchAsyncIsCalledOnGenericTaskResultWithFailure()
        {
            var error = Error.Validation("Invalid input");
            var resultTask = Task.FromResult(Result.Failure<int>(error));

            var output = await resultTask.MatchAsync(
                onSuccess: value => $"Value: {value}",
                onFailure: err => $"Error: {err.Description}"
            );

            Assert.Equal("Error: Invalid input", output);
        }

        #endregion

        #region MatchAsync Tests - Task<Result<T>> with Async Functions

        [Fact]
        public async Task Should_CallOnSuccessFunction_When_MatchAsyncIsCalledOnGenericTaskResultWithAsyncFunctions()
        {
            var resultTask = Task.FromResult(Result.Success("Hello"));

            var output = await resultTask.MatchAsync(
                onSuccess: async value =>
                {
                    await Task.Delay(1);
                    return $"Message: {value}";
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    return $"Error: {error.Description}";
                }
            );

            Assert.Equal("Message: Hello", output);
        }

        [Fact]
        public async Task Should_CallOnFailureFunction_When_MatchAsyncIsCalledOnGenericTaskResultWithAsyncFunctionsAndFailure()
        {
            var error = Error.Forbidden("Access denied");
            var resultTask = Task.FromResult(Result.Failure<string>(error));

            var output = await resultTask.MatchAsync(
                onSuccess: async value =>
                {
                    await Task.Delay(1);
                    return $"Message: {value}";
                },
                onFailure: async err =>
                {
                    await Task.Delay(1);
                    return $"Error: {err.Description}";
                }
            );

            Assert.Equal("Error: Access denied", output);
        }

        #endregion

        #region MatchAsync Tests - Complex Scenarios

        [Fact]
        public async Task Should_TransformToAnyType_When_GenericMatchAsyncIsUsed()
        {
            var result = Result.Success(42);

            var transformed = await result.MatchAsync(
                onSuccess: async value =>
                {
                    await Task.Delay(1);
                    return new { Number = value, IsEven = value % 2 == 0 };
                },
                onFailure: async error =>
                {
                    await Task.Delay(1);
                    return new { Number = 0, IsEven = false };
                }
            );

            Assert.Equal(42, transformed.Number);
            Assert.True(transformed.IsEven);
        }

        [Fact]
        public async Task Should_HandleComplexAsyncOperation_When_GenericMatchAsyncIsUsed()
        {
            var result = Result.Success("test@example.com");

            var output = await result.MatchAsync(
                onSuccess: async email =>
                {
                    // Simulate async database operation
                    await Task.Delay(10);
                    return new { Email = email, Domain = email.Split('@')[1] };
                },
                onFailure: async error =>
                {
                    await Task.Delay(10);
                    return new { Email = "unknown", Domain = "unknown" };
                }
            );

            Assert.Equal("test@example.com", output.Email);
            Assert.Equal("example.com", output.Domain);
        }

        [Fact]
        public async Task Should_WorkWithChainedAsyncOperations_When_MatchAsyncIsUsed()
        {
            var result = await GetUserAsync(1)
                .MatchAsync(
                    onSuccess: user => $"User found: {user}",
                    onFailure: error => $"Error: {error.Description}"
                );

            Assert.Equal("User found: John", result);
        }

        private static async Task<Result<string>> GetUserAsync(int id)
        {
            await Task.Delay(1);
            return Result.Success("John");
        }

        #endregion
    }
}
