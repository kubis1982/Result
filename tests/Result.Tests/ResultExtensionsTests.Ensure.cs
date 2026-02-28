using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class EnsureTests
    {
        #region Result<T> Tests

        [Fact]
        public void Should_ReturnOriginalResult_When_ResultIsSuccessAndPredicateIsTrue()
        {
            // Arrange
            var result = Result.Success(42);

            // Act
            var ensured = result.Ensure(x => x > 0, Error.Validation("Value must be positive"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        [Fact]
        public void Should_ReturnFailure_When_ResultIsSuccessAndPredicateIsFalse()
        {
            // Arrange
            var result = Result.Success(42);
            var error = Error.Validation("Value must be less than 10");

            // Act
            var ensured = result.Ensure(x => x < 10, error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
        }

        [Fact]
        public void Should_PropagateFailure_When_ResultIsAlreadyFailure()
        {
            // Arrange
            var originalError = Error.NotFound("Original error");
            var result = Result.Failure<int>(originalError);
            var validationError = Error.Validation("This should not be used");

            // Act
            var ensured = result.Ensure(x => x > 0, validationError);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(originalError, ensured.Error);
        }

        [Fact]
        public void Should_NotCallPredicate_When_ResultIsFailure()
        {
            // Arrange
            var result = Result.Failure<int>(Error.NotFound("Not found"));
            var predicateCalled = false;

            // Act
            var ensured = result.Ensure(x =>
            {
                predicateCalled = true;
                return true;
            }, Error.Validation("Validation error"));

            // Assert
            Assert.False(predicateCalled);
            Assert.True(ensured.IsFailure);
        }

        [Fact]
        public void Should_ChainMultipleValidations_When_AllPredicatesPass()
        {
            // Arrange
            var result = Result.Success(42);

            // Act
            var ensured = result
                .Ensure(x => x > 0, Error.Validation("Value must be positive"))
                .Ensure(x => x < 100, Error.Validation("Value must be less than 100"))
                .Ensure(x => x % 2 == 0, Error.Validation("Value must be even"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(42, ensured.Value);
        }

        [Fact]
        public void Should_StopAtFirstFailedValidation_When_ChainedValidations()
        {
            // Arrange
            var result = Result.Success(42);
            var thirdPredicateCalled = false;

            // Act
            var ensured = result
                .Ensure(x => x > 0, Error.Validation("Value must be positive"))
                .Ensure(x => x < 10, Error.Validation("Value must be less than 10"))
                .Ensure(x =>
                {
                    thirdPredicateCalled = true;
                    return x % 2 == 0;
                }, Error.Validation("Value must be even"));

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal("Value must be less than 10", ensured.Error.Description);
            Assert.False(thirdPredicateCalled);
        }

        [Fact]
        public void Should_WorkWithComplexObjects_When_ValidatingProperties()
        {
            // Arrange
            var user = new User { Name = "John", Age = 25, IsActive = true };
            var result = Result.Success(user);

            // Act
            var ensured = result
                .Ensure(u => u.IsActive, Error.Validation("User is not active"))
                .Ensure(u => u.Age >= 18, Error.Validation("User must be an adult"))
                .Ensure(u => !string.IsNullOrEmpty(u.Name), Error.Validation("User must have a name"));

            // Assert
            Assert.True(ensured.IsSuccess);
            Assert.Equal(user, ensured.Value);
        }

        [Fact]
        public void Should_ReturnValidationError_When_ComplexObjectValidationFails()
        {
            // Arrange
            var user = new User { Name = "John", Age = 15, IsActive = true };
            var result = Result.Success(user);
            var error = Error.Validation("User must be an adult");

            // Act
            var ensured = result
                .Ensure(u => u.IsActive, Error.Validation("User is not active"))
                .Ensure(u => u.Age >= 18, error);

            // Assert
            Assert.True(ensured.IsFailure);
            Assert.Equal(error, ensured.Error);
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
        public void Should_NotCallPredicate_When_NonGenericResultIsFailure()
        {
            // Arrange
            var result = Result.Failure(Error.NotFound("Not found"));
            var predicateCalled = false;

            // Act
            var ensured = result.Ensure(() =>
            {
                predicateCalled = true;
                return true;
            }, Error.Validation("Validation error"));

            // Assert
            Assert.False(predicateCalled);
            Assert.True(ensured.IsFailure);
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

        private record User
        {
            public required string Name { get; init; }
            public required int Age { get; init; }
            public required bool IsActive { get; init; }
        }
    }
}
