using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class MatchTests
    {

        #region Match Tests - Non-Generic Result

        [Fact]
        public void Should_CallOnSuccessFunction_When_MatchIsCalledOnNonGenericSuccess()
        {
            var result = Result.Success();

            var output = result.Match(
                onSuccess: () => "Success",
                onFailure: error => $"Error: {error.Description}"
            );

            Assert.Equal("Success", output);
        }

        [Fact]
        public void Should_CallOnFailureFunction_When_MatchIsCalledOnNonGenericFailure()
        {
            var error = Error.NotFound("Resource not found");
            var result = Result.Failure(error);

            var output = result.Match(
                onSuccess: () => "Success",
                onFailure: err => $"Error: {err.Description}"
            );

            Assert.Equal("Error: Resource not found", output);
        }

        [Fact]
        public void Should_NotCallOnFailure_When_NonGenericMatchOnSuccess()
        {
            var result = Result.Success();
            var onFailureCalled = false;

            result.Match(
                onSuccess: () => "Success",
                onFailure: error =>
                {
                    onFailureCalled = true;
                    return "Failure";
                }
            );

            Assert.False(onFailureCalled);
        }

        [Fact]
        public void Should_NotCallOnSuccess_When_NonGenericMatchOnFailure()
        {
            var result = Result.Failure(Error.Validation("Validation error"));
            var onSuccessCalled = false;

            result.Match(
                onSuccess: () =>
                {
                    onSuccessCalled = true;
                    return "Success";
                },
                onFailure: error => "Failure"
            );

            Assert.False(onSuccessCalled);
        }

        #endregion

        #region Match Tests - Generic Result

        [Fact]
        public void Should_CallOnSuccessFunctionWithValue_When_MatchIsCalledOnGenericSuccess()
        {
            var result = Result.Success(42);

            var output = result.Match(
                onSuccess: value => $"Value: {value}",
                onFailure: error => $"Error: {error.Description}"
            );

            Assert.Equal("Value: 42", output);
        }

        [Fact]
        public void Should_CallOnFailureFunctionWithError_When_MatchIsCalledOnGenericFailure()
        {
            var error = Error.NotFound("User not found");
            var result = Result.Failure<int>(error);

            var output = result.Match(
                onSuccess: value => $"Value: {value}",
                onFailure: err => $"Error: {err.Description}"
            );

            Assert.Equal("Error: User not found", output);
        }

        [Fact]
        public void Should_NotCallOnFailure_When_GenericMatchOnSuccess()
        {
            var result = Result.Success(100);
            var onFailureCalled = false;

            result.Match(
                onSuccess: value => value.ToString(),
                onFailure: error =>
                {
                    onFailureCalled = true;
                    return "Failure";
                }
            );

            Assert.False(onFailureCalled);
        }

        [Fact]
        public void Should_NotCallOnSuccess_When_GenericMatchOnFailure()
        {
            var result = Result.Failure<string>(Error.Unauthorized("Unauthorized"));
            var onSuccessCalled = false;

            result.Match(
                onSuccess: value =>
                {
                    onSuccessCalled = true;
                    return value;
                },
                onFailure: error => "Failure"
            );

            Assert.False(onSuccessCalled);
        }

        [Fact]
        public void Should_TransformToAnyType_When_GenericMatchIsUsed()
        {
            var result = Result.Success(42);

            var transformed = result.Match(
                onSuccess: value => new { Number = value, IsEven = value % 2 == 0 },
                onFailure: error => new { Number = 0, IsEven = false }
            );

            Assert.Equal(42, transformed.Number);
            Assert.True(transformed.IsEven);
        }

        [Fact]
        public void Should_ProcessComplexType_When_GenericMatchOnSuccess()
        {
            var user = new TestUser { Id = 1, Name = "John Doe" };
            var result = Result.Success(user);

            var output = result.Match(
                onSuccess: u => $"Welcome, {u.Name}!",
                onFailure: error => "Login failed"
            );

            Assert.Equal("Welcome, John Doe!", output);
        }

        [Fact]
        public void Should_ProcessComplexType_When_GenericMatchOnFailure()
        {
            var error = Error.NotFound("USER_NOT_FOUND", "User does not exist");
            var result = Result.Failure<TestUser>(error);

            var output = result.Match(
                onSuccess: user => $"Welcome, {user.Name}!",
                onFailure: err => $"Error [{err.Code}]: {err.Description}"
            );

            Assert.Equal("Error [USER_NOT_FOUND]: User does not exist", output);
        }

        #endregion

        #region Match Integration Tests

        [Fact]
        public void Should_WorkInReturnStatements_When_MatchIsUsed()
        {
            static string ProcessResult(Result<int> result)
            {
                return result.Match(
                    onSuccess: value => $"Processed: {value}",
                    onFailure: error => $"Failed: {error.Description}"
                );
            }

            var successResult = Result.Success(42);
            var failureResult = Result.Failure<int>(Error.Validation("Invalid"));

            Assert.Equal("Processed: 42", ProcessResult(successResult));
            Assert.Equal("Failed: Invalid", ProcessResult(failureResult));
        }

        [Fact]
        public void Should_EnforceExhaustiveHandling_When_MatchIsUsed()
        {
            // This test demonstrates that Match forces handling of both cases
            var result = Result.Success(100);

            // Both functions must be provided - compile-time safety
            var output = result.Match(
                onSuccess: value => value * 2,
                onFailure: error => 0
            );

            Assert.Equal(200, output);
        }

        [Fact]
        public void Should_ChainWithOtherOperations_When_MatchIsUsed()
        {
            var result = Result.Success(10)
                .Map(x => x * 2)
                .Match(
                    onSuccess: value => $"Result: {value}",
                    onFailure: error => $"Error: {error.Description}"
                );

            Assert.Equal("Result: 20", result);
        }

        [Fact]
        public void Should_MapToHttpResponse_When_MatchIsUsed()
        {
            var successResult = Result.Success(new { UserId = 123, Name = "John" });
            var failureResult = Result.Failure<object>(Error.NotFound("User not found"));

            var successResponse = successResult.Match(
                onSuccess: value => (StatusCode: 200, Body: value),
                onFailure: error => (StatusCode: 404, Body: (object)error)
            );

            var failureResponse = failureResult.Match(
                onSuccess: value => (StatusCode: 200, Body: value),
                onFailure: error => (StatusCode: error.ErrorType == ErrorType.NotFound ? 404 : 400, Body: (object)error)
            );

            Assert.Equal(200, successResponse.StatusCode);
            Assert.Equal(404, failureResponse.StatusCode);
        }

        [Fact]
        public void Should_WorkWithSwitchExpression_When_MatchIsUsed()
        {
            var result = Result.Success(42);

            // Match enforces handling similar to switch expressions
            var category = result.Match(
                onSuccess: value => value switch
                {
                    < 0 => "Negative",
                    0 => "Zero",
                    > 0 and <= 100 => "Positive Small",
                    _ => "Positive Large"
                },
                onFailure: error => "Error"
            );

            Assert.Equal("Positive Small", category);
        }

        [Fact]
        public void Should_ChainWithBind_When_NonGenericMatchIsUsed()
        {
            var result = Result.Success()
                .Bind(() => Result.Success())
                .Match(
                    onSuccess: () => "All operations succeeded",
                    onFailure: error => $"Failed: {error.Description}"
                );

            Assert.Equal("All operations succeeded", result);
        }

        [Fact]
        public void Should_HandleDifferentErrorTypes_When_GenericMatchIsUsed()
        {
            var notFoundResult = Result.Failure<int>(Error.NotFound("Not found"));
            var validationResult = Result.Failure<int>(Error.Validation("Invalid"));

            var notFoundMessage = notFoundResult.Match(
                onSuccess: value => $"Value: {value}",
                onFailure: error => $"[{error.ErrorType}] {error.Description}"
            );

            var validationMessage = validationResult.Match(
                onSuccess: value => $"Value: {value}",
                onFailure: error => $"[{error.ErrorType}] {error.Description}"
            );

            Assert.Equal("[NotFound] Not found", notFoundMessage);
            Assert.Equal("[Validation] Invalid", validationMessage);
        }

        #endregion

        private class TestUser
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
