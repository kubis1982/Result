using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class EnsureAsyncTests
    {
        #region Result<T> with async predicate Tests

        [Fact]
        public async Task Should_ReturnOriginalResult_When_ResultIsSuccessAndAsyncPredicateIsTrue()
        {
            // Arrange
            var result = Result.Success(42);

            // Act
            var ensured = await result.EnsureAsync(
                async x =>
                {
                    await Task.Delay(1);
                    return x > 0;
                },
                Error.Validation("Value must be positive"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ResultIsSuccessAndAsyncPredicateIsFalse()
        {
            // Arrange
            var result = Result.Success(42);
            var error = Error.Validation("Value must be less than 10");

            // Act
            var ensured = await result.EnsureAsync(
                async x =>
                {
                    await Task.Delay(1);
                    return x < 10;
                },
                error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        [Fact]
        public async Task Should_PropagateFailure_When_ResultIsAlreadyFailureAndAsyncPredicateProvided()
        {
            // Arrange
            var originalError = Error.NotFound("Original error");
            var result = Result.Failure<int>(originalError);
            var validationError = Error.Validation("This should not be used");

            // Act
            var ensured = await result.EnsureAsync(
                async x =>
                {
                    await Task.Delay(1);
                    return x > 0;
                },
                validationError);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(originalError, ensured.Error);
        }

        [Fact]
        public async Task Should_NotCallAsyncPredicate_When_ResultIsFailure()
        {
            // Arrange
            var result = Result.Failure<int>(Error.NotFound("Not found"));
            var predicateCalled = false;

            // Act
            var ensured = await result.EnsureAsync(
                async x =>
                {
                    await Task.Delay(1);
                    predicateCalled = true;
                    return true;
                },
                Error.Validation("Validation error"));

            // Assert
            Assert.False(predicateCalled);
            Assert.True(ensured.IsFailure);
        }

        [Fact]
        public async Task Should_ChainMultipleAsyncValidations_When_AllPredicatesPass()
        {
            // Arrange
            var result = Result.Success(42);

            // Act
            var ensured = await result
                .EnsureAsync(async x =>
                {
                    await Task.Delay(1);
                    return x > 0;
                }, Error.Validation("Value must be positive"))
                .EnsureAsync(async x =>
                {
                    await Task.Delay(1);
                    return x < 100;
                }, Error.Validation("Value must be less than 100"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        #endregion

        #region Task<Result<T>> with sync predicate Tests

        [Fact]
        public async Task Should_ReturnOriginalResult_When_TaskResultIsSuccessAndPredicateIsTrue()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success(42));

            // Act
            var ensured = await resultTask.EnsureAsync(
                x => x > 0,
                Error.Validation("Value must be positive"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_TaskResultIsSuccessAndPredicateIsFalse()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success(42));
            var error = Error.Validation("Value must be less than 10");

            // Act
            var ensured = await resultTask.EnsureAsync(x => x < 10, error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        [Fact]
        public async Task Should_PropagateFailure_When_TaskResultIsAlreadyFailure()
        {
            // Arrange
            var originalError = Error.NotFound("Original error");
            var resultTask = Task.FromResult(Result.Failure<int>(originalError));
            var validationError = Error.Validation("This should not be used");

            // Act
            var ensured = await resultTask.EnsureAsync(x => x > 0, validationError);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(originalError, ensured.Error);
        }

        #endregion

        #region Task<Result<T>> with async predicate Tests

        [Fact]
        public async Task Should_ReturnOriginalResult_When_TaskResultIsSuccessAndAsyncPredicateIsTrue()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success(42));

            // Act
            var ensured = await resultTask.EnsureAsync(
                async x =>
                {
                    await Task.Delay(1);
                    return x > 0;
                },
                Error.Validation("Value must be positive"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_TaskResultIsSuccessAndAsyncPredicateIsFalse()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success(42));
            var error = Error.Validation("Value must be less than 10");

            // Act
            var ensured = await resultTask.EnsureAsync(
                async x =>
                {
                    await Task.Delay(1);
                    return x < 10;
                },
                error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        [Fact]
        public async Task Should_ChainMixedSyncAndAsyncValidations_When_AllPredicatesPass()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success(42));

            // Act
            var ensured = await resultTask
                .EnsureAsync(x => x > 0, Error.Validation("Value must be positive"))
                .EnsureAsync(async x =>
                {
                    await Task.Delay(1);
                    return x < 100;
                }, Error.Validation("Value must be less than 100"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        #endregion

        #region Result (non-generic) Tests

        [Fact]
        public void Should_ReturnOriginalNonGenericResult_When_ResultIsSuccessAndPredicateIsTrue()
        {
            // Arrange
            var result = Result.Success();

            // Act
            var ensured = result.Ensure(() => true, Error.Validation("Validation failed"));

            // Assert
            Assert.True(ensured.IsSuccess);
        }

        [Fact]
        public void Should_ReturnFailure_When_NonGenericResultIsSuccessAndPredicateIsFalse()
        {
            // Arrange
            var result = Result.Success();
            var error = Error.Validation("Condition not met");

            // Act
            var ensured = result.Ensure(() => false, error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        [Fact]
        public void Should_PropagateFailure_When_NonGenericResultIsAlreadyFailure()
        {
            // Arrange
            var originalError = Error.Conflict("Original error");
            var result = Result.Failure(originalError);
            var validationError = Error.Validation("This should not be used");

            // Act
            var ensured = result.Ensure(() => true, validationError);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(originalError, ensured.Error);
        }

        [Fact]
        public void Should_ChainMultipleValidations_When_AllNonGenericPredicatesPass()
        {
            // Arrange
            var result = Result.Success();
            var condition1 = true;
            var condition2 = true;

            // Act
            var ensured = result
                .Ensure(() => condition1, Error.Validation("Condition 1 failed"))
                .Ensure(() => condition2, Error.Validation("Condition 2 failed"));

            // Assert
            Assert.True(ensured.IsSuccess);
        }

        #endregion

        #region Result (non-generic) Async Tests

        [Fact]
        public async Task Should_ReturnOriginalNonGenericResult_When_ResultIsSuccessAndAsyncPredicateIsTrue()
        {
            // Arrange
            var result = Result.Success();

            // Act
            var ensured = await result.EnsureAsync(
                async () =>
                {
                    await Task.Delay(1);
                    return true;
                },
                Error.Validation("Validation failed"));

            // Assert
            Assert.True(ensured.IsSuccess);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_NonGenericResultIsSuccessAndAsyncPredicateIsFalse()
        {
            // Arrange
            var result = Result.Success();
            var error = Error.Validation("Condition not met");

            // Act
            var ensured = await result.EnsureAsync(
                async () =>
                {
                    await Task.Delay(1);
                    return false;
                },
                error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        [Fact]
        public async Task Should_PropagateFailure_When_NonGenericResultIsAlreadyFailureWithAsyncPredicate()
        {
            // Arrange
            var originalError = Error.Conflict("Original error");
            var result = Result.Failure(originalError);
            var validationError = Error.Validation("This should not be used");

            // Act
            var ensured = await result.EnsureAsync(
                async () =>
                {
                    await Task.Delay(1);
                    return true;
                },
                validationError);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(originalError, ensured.Error);
        }

        [Fact]
        public async Task Should_NotCallAsyncPredicate_When_NonGenericResultIsFailure()
        {
            // Arrange
            var result = Result.Failure(Error.NotFound("Not found"));
            var predicateCalled = false;

            // Act
            var ensured = await result.EnsureAsync(
                async () =>
                {
                    await Task.Delay(1);
                    predicateCalled = true;
                    return true;
                },
                Error.Validation("Validation error"));

            // Assert
            Assert.False(predicateCalled);
            Assert.True(ensured.IsFailure);
        }

        #endregion

        #region Task<Result> (non-generic) Tests

        [Fact]
        public async Task Should_ReturnOriginalResult_When_TaskNonGenericResultIsSuccessAndPredicateIsTrue()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success());

            // Act
            var ensured = await resultTask.EnsureAsync(() => true, Error.Validation("Validation failed"));

            // Assert
            Assert.True(ensured.IsSuccess);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_TaskNonGenericResultIsSuccessAndPredicateIsFalse()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success());
            var error = Error.Validation("Condition not met");

            // Act
            var ensured = await resultTask.EnsureAsync(() => false, error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        [Fact]
        public async Task Should_ReturnOriginalResult_When_TaskNonGenericResultIsSuccessAndAsyncPredicateIsTrue()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success());

            // Act
            var ensured = await resultTask.EnsureAsync(
                async () =>
                {
                    await Task.Delay(1);
                    return true;
                },
                Error.Validation("Validation failed"));

            // Assert
            Assert.True(ensured.IsSuccess);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_TaskNonGenericResultIsSuccessAndAsyncPredicateIsFalse()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success());
            var error = Error.Validation("Condition not met");

            // Act
            var ensured = await resultTask.EnsureAsync(
                async () =>
                {
                    await Task.Delay(1);
                    return false;
                },
                error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        #endregion

        #region Complex Chaining Scenarios

        [Fact]
        public async Task Should_WorkInComplexChain_When_MixingMapAndEnsure()
        {
            // Arrange
            var user = new User { Name = "John", Age = 25, IsActive = true };
            var result = Result.Success(user);

            // Act
            var ensured = await result
                .Ensure(u => u.IsActive, Error.Validation("User is not active"))
                .EnsureAsync(
                    async u =>
                    {
                        await Task.Delay(1);
                        return u.Age >= 18;
                    },
                    Error.Validation("User must be an adult"))
                .MapAsync(async u =>
                {
                    await Task.Delay(1);
                    return u.Name;
                });

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal("John", ensured.Value);
        }

        [Fact]
        public async Task Should_StopAtFirstFailedValidation_When_ChainedAsyncValidations()
        {
            // Arrange
            var result = Result.Success(42);
            var thirdPredicateCalled = false;

            // Act
            var ensured = await result
                .EnsureAsync(
                    async x =>
                    {
                        await Task.Delay(1);
                        return x > 0;
                    },
                    Error.Validation("Value must be positive"))
                .EnsureAsync(
                    async x =>
                    {
                        await Task.Delay(1);
                        return x < 10;
                    },
                    Error.Validation("Value must be less than 10"))
                .EnsureAsync(
                    async x =>
                    {
                        await Task.Delay(1);
                        thirdPredicateCalled = true;
                        return x % 2 == 0;
                    },
                    Error.Validation("Value must be even"));

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal("Value must be less than 10", ensured.Error.Description);
            Assert.False(thirdPredicateCalled);
        }

        [Fact]
        public async Task Should_WorkWithTaskResultAndMixedPredicates_When_ChainedValidations()
        {
            // Arrange
            var resultTask = Task.FromResult(Result.Success(42));

            // Act
            var ensured = await resultTask
                .EnsureAsync(x => x > 0, Error.Validation("Value must be positive"))
                .EnsureAsync(
                    async x =>
                    {
                        await Task.Delay(1);
                        return x < 100;
                    },
                    Error.Validation("Value must be less than 100"))
                .EnsureAsync(x => x % 2 == 0, Error.Validation("Value must be even"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        [Fact]
        public async Task Should_ValidateAsyncDatabaseCheck_When_UsingRealWorldScenario()
        {
            // Arrange
            var userId = 123;
            var result = Result.Success(userId);

            // Act
            var ensured = await result
                .Ensure(id => id > 0, Error.Validation("Invalid user ID"))
                .EnsureAsync(
                    async id =>
                    {
                        await Task.Delay(1); // Simulate DB call
                        return id != 999; // Simulating user exists check
                    },
                    Error.NotFound("User not found"))
                .EnsureAsync(
                    async id =>
                    {
                        await Task.Delay(1); // Simulate another DB call
                        return id != 456; // Simulating user not banned check
                    },
                    Error.Forbidden("User is banned"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(123, ensured.Value);
        }

        #endregion

        private record User
        {
            public required string Name { get; init; }
            public required int Age { get; init; }
            public required bool IsActive { get; init; }
        }
    }
}
