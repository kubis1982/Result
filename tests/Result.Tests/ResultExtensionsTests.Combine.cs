using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class CombineTests
    {
        #region Combine Tests

        [Fact]
        public void Combine_WithAllSuccessfulResults_ReturnsSuccess()
        {
            var result1 = Result.Success();
            var result2 = Result.Success();
            var result3 = Result.Success();

            var combined = ResultExtensions.Combine(result1, result2, result3);

            Assert.True(combined.IsSuccess);
        }

        [Fact]
        public void Combine_WithOneFailure_ReturnsFirstFailure()
        {
            var result1 = Result.Success();
            var error = Error.NotFound("not found");
            var result2 = Result.Failure(error);
            var result3 = Result.Success();

            var combined = ResultExtensions.Combine(result1, result2, result3);

            Assert.False(combined.IsSuccess);
            Assert.Equal(error, combined.Error);
        }

        [Fact]
        public void Combine_WithMultipleFailures_ReturnsFirstFailure()
        {
            var result1 = Result.Success();
            var error1 = Error.NotFound("not found");
            var result2 = Result.Failure(error1);
            var error2 = Error.Validation("validation error");
            var result3 = Result.Failure(error2);

            var combined = ResultExtensions.Combine(result1, result2, result3);

            Assert.False(combined.IsSuccess);
            Assert.Equal(error1, combined.Error);
        }

        [Fact]
        public void Combine_WithEmptyArray_ReturnsSuccess()
        {
            var combined = ResultExtensions.Combine();

            Assert.True(combined.IsSuccess);
        }

        [Fact]
        public void Combine_WithIEnumerable_AllSuccessful_ReturnsSuccess()
        {
            var results = new List<Result>
        {
            Result.Success(),
            Result.Success(),
            Result.Success()
        };

            var combined = ResultExtensions.Combine(results);

            Assert.True(combined.IsSuccess);
        }

        [Fact]
        public void Combine_WithIEnumerable_OneFailure_ReturnsFirstFailure()
        {
            var error = Error.Forbidden("forbidden");
            var results = new List<Result>
        {
            Result.Success(),
            Result.Failure(error),
            Result.Success()
        };

            var combined = ResultExtensions.Combine(results);

            Assert.False(combined.IsSuccess);
            Assert.Equal(error, combined.Error);
        }

        [Fact]
        public void Combine_Generic_WithAllSuccessfulResults_ReturnsSuccessWithValues()
        {
            var result1 = Result.Success(10);
            var result2 = Result.Success(20);
            var result3 = Result.Success(30);

            var combined = ResultExtensions.Combine(result1, result2, result3);

            Assert.True(combined.IsSuccess);
            Assert.Equal([10, 20, 30], combined.Value);
        }

        [Fact]
        public void Combine_Generic_WithOneFailure_ReturnsFirstFailure()
        {
            var result1 = Result.Success(10);
            var error = Error.NotFound("not found");
            var result2 = Result.Failure<int>(error);
            var result3 = Result.Success(30);

            var combined = ResultExtensions.Combine(result1, result2, result3);

            Assert.False(combined.IsSuccess);
            Assert.Equal(error, combined.Error);
        }

        [Fact]
        public void Combine_Generic_WithMultipleFailures_ReturnsFirstFailure()
        {
            var result1 = Result.Success(10);
            var error1 = Error.Validation("validation error");
            var result2 = Result.Failure<int>(error1);
            var error2 = Error.Unauthorized("unauthorized");
            var result3 = Result.Failure<int>(error2);

            var combined = ResultExtensions.Combine(result1, result2, result3);

            Assert.False(combined.IsSuccess);
            Assert.Equal(error1, combined.Error);
        }

        [Fact]
        public void Combine_Generic_WithEmptyArray_ReturnsSuccessWithEmptyCollection()
        {
            var combined = ResultExtensions.Combine<int>();

            Assert.True(combined.IsSuccess);
            Assert.Empty(combined.Value);
        }

        [Fact]
        public void Combine_Generic_WithIEnumerable_AllSuccessful_ReturnsSuccessWithValues()
        {
            var results = new List<Result<string>>
        {
            Result.Success("a"),
            Result.Success("b"),
            Result.Success("c")
        };

            var combined = ResultExtensions.Combine(results);

            Assert.True(combined.IsSuccess);
            Assert.Equal(["a", "b", "c"], combined.Value);
        }

        [Fact]
        public void Combine_Generic_WithIEnumerable_OneFailure_ReturnsFirstFailure()
        {
            var error = Error.Conflict("conflict");
            var results = new List<Result<string>>
        {
            Result.Success("a"),
            Result.Failure<string>(error),
            Result.Success("c")
        };

            var combined = ResultExtensions.Combine(results);

            Assert.False(combined.IsSuccess);
            Assert.Equal(error, combined.Error);
        }

        [Fact]
        public void Combine_Generic_CanProcessResultsInPipeline()
        {
            var result1 = Result.Success(10);
            var result2 = Result.Success(20);
            var result3 = Result.Success(30);

            var combined = ResultExtensions.Combine(result1, result2, result3)
                .Map(values => values.Sum())
                .Map(sum => $"Total: {sum}");

            Assert.True(combined.IsSuccess);
            Assert.Equal("Total: 60", combined.Value);
        }

        [Fact]
        public void Combine_Generic_FailFastStopsEvaluation()
        {
            var result1 = Result.Success(10);
            var result2 = Result.Validation<int>("validation failed");
            var evaluationCounter = 0;
            var result3 = Result.Success(30).Map(x => { evaluationCounter++; return x; });

            var results = new List<Result<int>> { result1, result2, result3 };
            var combined = ResultExtensions.Combine(results);

            Assert.False(combined.IsSuccess);
            Assert.Equal(result2.Error, combined.Error);
            Assert.Equal(1, evaluationCounter);
        }

        [Fact]
        public void Combine_CanBeUsedForValidation()
        {
            string name = "John";
            string email = "john@example.com";
            int age = 25;

            var nameValidation = ValidateName(name);
            var emailValidation = ValidateEmail(email);
            var ageValidation = ValidateAge(age);

            var validationResult = ResultExtensions.Combine(nameValidation, emailValidation, ageValidation);

            Assert.True(validationResult.IsSuccess);

            static Result ValidateName(string name) =>
                string.IsNullOrEmpty(name)
                    ? Result.Validation("Name is required")
                    : Result.Success();

            static Result ValidateEmail(string email) =>
                email.Contains('@')
                    ? Result.Success()
                    : Result.Validation("Invalid email");

            static Result ValidateAge(int age) =>
                age >= 18
                    ? Result.Success()
                    : Result.Validation("Must be 18 or older");
        }

        [Fact]
        public void Combine_ValidationScenario_ReturnsFirstValidationError()
        {
            string name = "";
            string email = "invalid-email";
            int age = 15;

            var nameValidation = ValidateName(name);
            var emailValidation = ValidateEmail(email);
            var ageValidation = ValidateAge(age);

            var validationResult = ResultExtensions.Combine(nameValidation, emailValidation, ageValidation);

            Assert.False(validationResult.IsSuccess);
            Assert.Equal("Name is required", validationResult.Error.Description);

            static Result ValidateName(string name) =>
                string.IsNullOrEmpty(name)
                    ? Result.Validation("Name is required")
                    : Result.Success();

            static Result ValidateEmail(string email) =>
                email.Contains('@')
                    ? Result.Success()
                    : Result.Validation("Invalid email");

            static Result ValidateAge(int age) =>
                age >= 18
                    ? Result.Success()
                    : Result.Validation("Must be 18 or older");
        }

        #endregion
    }
}
