using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class BindTests
    {

        #region Bind<T, TOut> Tests

        [Fact]
        public void Should_ReturnBinderResult_When_BindIsCalledOnSuccess()
        {
            var result = Result.Success(42);

            var bound = result.Bind(x => Result.Success(x.ToString()));

            Assert.True(bound.IsSuccess);
            Assert.Equal("42", bound.Value);
        }

        [Fact]
        public void Should_PropagateFailure_When_BindIsCalledOnFailure()
        {
            var result = Result.NotFound<int>("not found");

            var bound = result.Bind(x => Result.Success(x.ToString()));

            Assert.True(bound.IsFailure);
            Assert.Equal(result.Error, bound.Error);
        }

        [Fact]
        public void Should_ReturnFailureFromBinder_When_BindIsUsed()
        {
            var result = Result.Success(42);
            var resultError = Result.Validation("validation failed");

            var bound = result.Bind(x => resultError);

            Assert.True(bound.IsFailure);
            Assert.Equal(resultError.Error, bound.Error);  
        }

        [Fact]
        public void Should_ExecuteBinderOnlyForSuccess_When_BindIsUsed()
        {
            var error = Error.Validation("validation error");
            var result = Result.Failure<int>(error);
            var binderCalled = false;

            var bound = result.Bind(x =>
            {
                binderCalled = true;
                return Result.Success(x.ToString());
            });

            Assert.False(binderCalled);
            Assert.True(bound.IsFailure);
        }

        [Fact]
        public void Should_ChainOperations_When_BindIsUsed()
        {
            var result = Result.Success(10);

            var bound = result
                .Bind(x => Result.Success(x * 2))
                .Bind(x => Result.Success(x + 5))
                .Bind(x => Result.Success(x.ToString()));

            Assert.True(bound.IsSuccess);
            Assert.Equal("25", bound.Value);
        }

        [Fact]
        public void Should_StopAtFirstFailure_When_ChainingBind()
        {
            var result = Result.Success(10);
            var error = Error.Validation("failed");

            var bound = result
                .Bind(x => Result.Success(x * 2))
                .Bind(x => Result.Failure<int>(error))
                .Bind(x => Result.Success<string>("should not execute"));

            Assert.True(bound.IsFailure);
            Assert.Equal(error, bound.Error);
        }

        #endregion

        #region Bind<TSource> to Result Tests

        [Fact]
        public void Should_ReturnBinderResult_When_BindToNonGenericIsCalledOnSuccess()
        {
            var result = Result.Success(42);

            var bound = result.Bind(x => Result.Success());

            Assert.True(bound.IsSuccess);
        }

        [Fact]
        public void Should_PropagateFailure_When_BindToNonGenericIsCalledOnFailure()
        {
            var error = Error.Validation("E001", "custom error");
            var result = Result.Failure<string>(error);

            var bound = result.Bind(x => Result.Success());

            Assert.True(bound.IsFailure);
            Assert.Equal(error, bound.Error);
        }

        [Fact]
        public void Should_ReturnFailureFromBinder_When_BindToNonGenericIsUsed()
        {
            var result = Result.Success(42);
            var binderError = Error.Unauthorized("unauthorized");

            var bound = result.Bind(x => Result.Failure(binderError));

            Assert.True(bound.IsFailure);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public void Should_ExecuteBinderOnlyForSuccess_When_BindToNonGenericIsUsed()
        {
            var error = Error.Validation("validation error");
            var result = Result.Failure<int>(error);
            var binderCalled = false;

            var bound = result.Bind(x =>
            {
                binderCalled = true;
                return Result.Success();
            });

            Assert.False(binderCalled);
            Assert.True(bound.IsFailure);
        }

        #endregion

        #region Bind<TOut> from Result Tests

        [Fact]
        public void Should_ReturnBinderResult_When_BindFromNonGenericToGenericIsCalledOnSuccess()
        {
            var result = Result.Success();

            var bound = result.Bind(() => Result.Success("hello"));

            Assert.True(bound.IsSuccess);
            Assert.Equal("hello", bound.Value);
        }

        [Fact]
        public void Should_PropagateFailure_When_BindFromNonGenericToGenericIsCalledOnFailure()
        {
            var error = Error.Forbidden("access denied");
            var result = Result.Failure(error);

            var bound = result.Bind(() => Result.Success("hello"));

            Assert.True(bound.IsFailure);
            Assert.Equal(error, bound.Error);
        }

        [Fact]
        public void Should_ReturnFailureFromBinder_When_BindFromNonGenericToGenericIsUsed()
        {
            var result = Result.Success();
            var binderError = Error.NotFound("not found");

            var bound = result.Bind(() => Result.Failure(binderError));

            Assert.True(bound.IsFailure);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public void Should_ExecuteBinderOnlyForSuccess_When_BindFromNonGenericToGenericIsUsed()
        {
            var error = Error.Validation("validation error");
            var result = Result.Failure(error);
            var binderCalled = false;

            var bound = result.Bind(() =>
            {
                binderCalled = true;
                return Result.Success("value");
            });

            Assert.False(binderCalled);
            Assert.True(bound.IsFailure);
        }

        #endregion

        #region Bind from Result to Result Tests

        [Fact]
        public void Should_ReturnBinderResult_When_BindNonGenericToNonGenericIsCalledOnSuccess()
        {
            var result = Result.Success();

            var bound = result.Bind(() => Result.Success());

            Assert.True(bound.IsSuccess);
        }

        [Fact]
        public void Should_PropagateFailure_When_BindNonGenericToNonGenericIsCalledOnFailure()
        {
            var result = Result.Validation("SERVER.ERROR", "server error");

            var bound = result.Bind(() => Result.Success());

            Assert.True(bound.IsFailure);
            Assert.Equal(result.Error, bound.Error);
        }

        [Fact]
        public void Should_ReturnFailureFromBinder_When_BindNonGenericToNonGenericIsUsed()
        {
            var result = Result.Success();
            var binderError = Error.Conflict("conflict");

            var bound = result.Bind(() => Result.Failure(binderError));

            Assert.True(bound.IsFailure);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public void Should_ExecuteBinderOnlyForSuccess_When_BindNonGenericToNonGenericIsUsed()
        {
            var error = Error.Validation("validation error");
            var result = Result.Failure(error);
            var binderCalled = false;

            var bound = result.Bind(() =>
            {
                binderCalled = true;
                return Result.Success();
            });

            Assert.False(binderCalled);
            Assert.True(bound.IsFailure);
        }

        [Fact]
        public void Should_ChainMultipleOperations_When_BindNonGenericToNonGenericIsUsed()
        {
            var result = Result.Success();

            var bound = result
                .Bind(() => Result.Success())
                .Bind(() => Result.Success())
                .Bind(() => Result.Success());

            Assert.True(bound.IsSuccess);
        }

        #endregion

        #region Bind Integration Tests

        [Fact]
        public void Should_MixWithMapInChain_When_BindIsUsed()
        {
            var result = Result.Success(10);

            var final = result
                .Map(x => x * 2)
                .Bind(x => Result.Success(x + 5))
                .Map(x => x.ToString())
                .Bind(x => Result.Success($"Result: {x}"));

            Assert.True(final.IsSuccess);
            Assert.Equal("Result: 25", final.Value);
        }

        [Fact]
        public void Should_HandleComplexChainWithMultipleTransformations_When_BindIsUsed()
        {
            var result = Result.Success("10");

            var final = result
                .Bind(x => int.TryParse(x, out var num)
                    ? Result.Success(num)
                    : Result.Validation<int>("not a number"))
                .Bind(x => x > 0
                    ? Result.Success(x * 2)
                    : Result.Validation<int>("not positive"))
                .Map(x => $"Value: {x}");

            Assert.True(final.IsSuccess);
            Assert.Equal("Value: 20", final.Value);
        }

        [Fact]
        public void Should_StopAtFirstFailureInBind_When_UsingComplexChain()
        {
            var result = Result.Success("abc");

            var final = result
                .Bind(x => int.TryParse(x, out var num)
                    ? Result.Success(num)
                    : Result.Validation<int>("not a number"))
                .Bind(x => Result.Success(x * 2))
                .Map(x => $"Value: {x}");

            Assert.True(final.IsFailure);
            Assert.Equal(ErrorCodes.Validation, final.Error.Code);
        }

        #endregion
    }
}
