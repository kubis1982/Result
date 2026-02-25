using Xunit;

namespace Kubis1982.Result;

public partial class ResultExtensionsTests
{
    public class BindingTests
    {

        #region Bind<T, TOut> Tests

        [Fact]
        public void Bind_WithBinder_WhenResultIsSuccess_ReturnsBinderResult()
        {
            var result = Result.Success(42);

            var bound = result.Bind(x => Result.Success(x.ToString()));

            Assert.True(bound.IsSuccess);
            Assert.Equal("42", bound.Value);
        }

        [Fact]
        public void Bind_WithBinder_WhenResultIsFailure_PropagatesFailure()
        {
            var error = Error.NotFound("not found");
            var result = Result.Failure<int>(error);

            var bound = result.Bind(x => Result.Success(x.ToString()));

            Assert.False(bound.IsSuccess);
            Assert.Equal(error, bound.Error);
        }

        [Fact]
        public void Bind_WithBinder_CanReturnFailureFromBinder()
        {
            var result = Result<int>.Success(42);
            var binderError = Error.Validation("validation failed");

            var bound = result.Bind(x => Result<string>.Failure(binderError));

            Assert.False(bound.IsSuccess);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public void Bind_WithBinder_ExecutesBinderOnlyForSuccess()
        {
            var error = Error.Validation("validation error");
            var result = Result.Failure<int>(error);
            var binderCalled = false;

            var bound = result.Bind(x =>
            {
                binderCalled = true;
                return Result<string>.Success(x.ToString());
            });

            Assert.False(binderCalled);
            Assert.False(bound.IsSuccess);
        }

        [Fact]
        public void Bind_WithBinder_CanChainOperations()
        {
            var result = Result<int>.Success(10);

            var bound = result
                .Bind(x => Result.Success(x * 2))
                .Bind(x => Result.Success(x + 5))
                .Bind(x => Result.Success(x.ToString()));

            Assert.True(bound.IsSuccess);
            Assert.Equal("25", bound.Value);
        }

        [Fact]
        public void Bind_WithBinder_ChainStopsAtFirstFailure()
        {
            var result = Result.Success(10);
            var error = Error.Validation("failed");

            var bound = result
                .Bind(x => Result.Success(x * 2))
                .Bind(x => Result.Failure<int>(error))
                .Bind(x => Result.Success<string>("should not execute"));

            Assert.False(bound.IsSuccess);
            Assert.Equal(error, bound.Error);
        }

        #endregion

        #region Bind<TSource> to Result Tests

        [Fact]
        public void Bind_ToNonGeneric_WhenResultIsSuccess_ReturnsBinderResult()
        {
            var result = Result.Success(42);

            var bound = result.Bind(x => Result.Success());

            Assert.True(bound.IsSuccess);
        }

        [Fact]
        public void Bind_ToNonGeneric_WhenResultIsFailure_PropagatesFailure()
        {
            var error = Error.Validation("E001", "custom error");
            var result = Result.Failure<string>(error);

            var bound = result.Bind(x => Result.Success());

            Assert.False(bound.IsSuccess);
            Assert.Equal(error, bound.Error);
        }

        [Fact]
        public void Bind_ToNonGeneric_CanReturnFailureFromBinder()
        {
            var result = Result.Success(42);
            var binderError = Error.Unauthorized("unauthorized");

            var bound = result.Bind(x => Result.Failure(binderError));

            Assert.False(bound.IsSuccess);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public void Bind_ToNonGeneric_ExecutesBinderOnlyForSuccess()
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
            Assert.False(bound.IsSuccess);
        }

        #endregion

        #region Bind<TOut> from Result Tests

        [Fact]
        public void Bind_FromNonGenericToGeneric_WhenResultIsSuccess_ReturnsBinderResult()
        {
            var result = Result.Success();

            var bound = result.Bind(() => Result.Success("hello"));

            Assert.True(bound.IsSuccess);
            Assert.Equal("hello", bound.Value);
        }

        [Fact]
        public void Bind_FromNonGenericToGeneric_WhenResultIsFailure_PropagatesFailure()
        {
            var error = Error.Forbidden("access denied");
            var result = Result.Failure(error);

            var bound = result.Bind(() => Result.Success("hello"));

            Assert.False(bound.IsSuccess);
            Assert.Equal(error, bound.Error);
        }

        [Fact]
        public void Bind_FromNonGenericToGeneric_CanReturnFailureFromBinder()
        {
            var result = Result.Success();
            var binderError = Error.NotFound("not found");

            var bound = result.Bind(() => Result.Failure(binderError));

            Assert.False(bound.IsSuccess);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public void Bind_FromNonGenericToGeneric_ExecutesBinderOnlyForSuccess()
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
            Assert.False(bound.IsSuccess);
        }

        #endregion

        #region Bind from Result to Result Tests

        [Fact]
        public void Bind_NonGenericToNonGeneric_WhenResultIsSuccess_ReturnsBinderResult()
        {
            var result = Result.Success();

            var bound = result.Bind(() => Result.Success());

            Assert.True(bound.IsSuccess);
        }

        [Fact]
        public void Bind_NonGenericToNonGeneric_WhenResultIsFailure_PropagatesFailure()
        {
            var error = Error.Validation("SERVER.ERROR", "server error");
            var result = Result.Failure(error);

            var bound = result.Bind(() => Result.Success());

            Assert.False(bound.IsSuccess);
            Assert.Equal(error, bound.Error);
        }

        [Fact]
        public void Bind_NonGenericToNonGeneric_CanReturnFailureFromBinder()
        {
            var result = Result.Success();
            var binderError = Error.Conflict("conflict");

            var bound = result.Bind(() => Result.Failure(binderError));

            Assert.False(bound.IsSuccess);
            Assert.Equal(binderError, bound.Error);
        }

        [Fact]
        public void Bind_NonGenericToNonGeneric_ExecutesBinderOnlyForSuccess()
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
            Assert.False(bound.IsSuccess);
        }

        [Fact]
        public void Bind_NonGenericToNonGeneric_CanChainMultipleOperations()
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
        public void Bind_CanMixWithMapInChain()
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
        public void Bind_ComplexChainWithMultipleTransformations()
        {
            var result = Result.Success("10");

            var final = result
                .Bind(x => int.TryParse(x, out var num)
                    ? Result.Success(num)
                    : Result.Failure<int>(Error.Validation("not a number")))
                .Bind(x => x > 0
                    ? Result.Success(x * 2)
                    : Result.Failure<int>(Error.Validation("not positive")))
                .Map(x => $"Value: {x}");

            Assert.True(final.IsSuccess);
            Assert.Equal("Value: 20", final.Value);
        }

        [Fact]
        public void Bind_ComplexChainStopsAtFirstFailureInBind()
        {
            var result = Result.Success("abc");

            var final = result
                .Bind(x => int.TryParse(x, out var num)
                    ? Result.Success(num)
                    : Result.Validation<int>("not a number"))
                .Bind(x => Result.Success(x * 2))
                .Map(x => $"Value: {x}");

            Assert.False(final.IsSuccess);
            Assert.Equal(ErrorCodes.Validation, final.Error.Code);
        }

        #endregion
    }
}
