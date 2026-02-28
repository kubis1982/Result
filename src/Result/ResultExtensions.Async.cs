namespace Kubis1982.Result;

/// <summary>
/// Asynchronous extension methods for Result types.
/// </summary>
public static partial class ResultExtensions
{
    #region MapAsync

    /// <summary>
    /// Asynchronously projects the value of a successful <see cref="Result{T}"/> to a new value of type <typeparamref name="TOut"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="mapper">An asynchronous mapping function applied to the inner value when the result is successful.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{TOut}"/> with the mapped value when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Result<T> result, Func<T, Task<TOut>> mapper)
    {
        if (!result.IsSuccess)
        {
            return Result.Failure<TOut>(result.Error);
        }

        var mappedValue = await mapper(result.Value).ConfigureAwait(false);
        return Result.Success(mappedValue);
    }

    /// <summary>
    /// Projects the value of a successful result from an asynchronous <see cref="Task{Result}"/> to a new value of type <typeparamref name="TOut"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="mapper">A synchronous mapping function applied to the inner value when the result is successful.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{TOut}"/> with the mapped value when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Task<Result<T>> resultTask, Func<T, TOut> mapper)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Map(mapper);
    }

    /// <summary>
    /// Asynchronously projects the value of a successful result from an asynchronous <see cref="Task{Result}"/> to a new value of type <typeparamref name="TOut"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="mapper">An asynchronous mapping function applied to the inner value when the result is successful.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{TOut}"/> with the mapped value when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Task<Result<T>> resultTask, Func<T, Task<TOut>> mapper)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapAsync(mapper).ConfigureAwait(false);
    }

    /// <summary>
    /// Converts an asynchronous <see cref="Task{TResult}"/> containing <see cref="Result{T}"/> to a non-generic <see cref="Task{TResult}"/> containing <see cref="Result"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a non-generic <see cref="Result"/> with success status when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static async Task<Result> MapAsync<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Map();
    }

    #endregion

    #region BindAsync

    /// <summary>
    /// Asynchronously chains a function that produces a <see cref="Result{TOut}"/> based on the value of a successful <see cref="Result{T}"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="binder">An asynchronous function that takes the inner value and returns a new result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Result<T> result, Func<T, Task<Result<TOut>>> binder)
    {
        if (!result.IsSuccess)
        {
            return Result.Failure<TOut>(result.Error);
        }

        return await binder(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Chains a function that produces a result based on the value of a successful result from an asynchronous <see cref="Task{Result}"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">A synchronous function that takes the inner value and returns a new result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> resultTask, Func<T, Result<TOut>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(binder);
    }

    /// <summary>
    /// Asynchronously chains a function that produces a result based on the value of a successful result from an asynchronous <see cref="Task{Result}"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">An asynchronous function that takes the inner value and returns a new result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> resultTask, Func<T, Task<Result<TOut>>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(binder).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously chains a function that produces a non-generic <see cref="Result"/> based on the value of a successful <see cref="Result{T}"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="binder">An asynchronous function that takes the inner value and returns a new non-generic result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static async Task<Result> BindAsync<T>(this Result<T> result, Func<T, Task<Result>> binder)
    {
        if (!result.IsSuccess)
        {
            return result.Map();
        }

        return await binder(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Chains a function that produces a non-generic result based on the value of a successful result from an asynchronous <see cref="Task{Result}"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">A synchronous function that takes the inner value and returns a new non-generic result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static async Task<Result> BindAsync<T>(this Task<Result<T>> resultTask, Func<T, Result> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(binder);
    }

    /// <summary>
    /// Asynchronously chains a function that produces a non-generic result based on the value of a successful result from an asynchronous <see cref="Task{Result}"/>.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">An asynchronous function that takes the inner value and returns a new non-generic result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static async Task<Result> BindAsync<T>(this Task<Result<T>> resultTask, Func<T, Task<Result>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(binder).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously chains a function that produces a <see cref="Result{TOut}"/> when the current non-generic <see cref="Result"/> is successful.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="binder">An asynchronous function that returns a new result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> BindAsync<TOut>(this Result result, Func<Task<Result<TOut>>> binder)
    {
        if (!result.IsSuccess)
        {
            return result.Map<TOut>();
        }

        return await binder().ConfigureAwait(false);
    }

    /// <summary>
    /// Chains a function that produces a result when the current non-generic result from an asynchronous task is successful.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">A synchronous function that returns a new result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> BindAsync<TOut>(this Task<Result> resultTask, Func<Result<TOut>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(binder);
    }

    /// <summary>
    /// Asynchronously chains a function that produces a result when the current non-generic result from an asynchronous task is successful.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">An asynchronous function that returns a new result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static async Task<Result<TOut>> BindAsync<TOut>(this Task<Result> resultTask, Func<Task<Result<TOut>>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(binder).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously chains a function that produces a non-generic <see cref="Result"/> when the current non-generic <see cref="Result"/> is successful.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <param name="result">The source result.</param>
    /// <param name="binder">An asynchronous function that returns a new non-generic result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static async Task<Result> BindAsync(this Result result, Func<Task<Result>> binder)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        return await binder().ConfigureAwait(false);
    }

    /// <summary>
    /// Chains a function that produces a non-generic result when the current non-generic result from an asynchronous task is successful.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">A synchronous function that returns a new non-generic result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static async Task<Result> BindAsync(this Task<Result> resultTask, Func<Result> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(binder);
    }

    /// <summary>
    /// Asynchronously chains a function that produces a non-generic result when the current non-generic result from an asynchronous task is successful.
    /// If the current result is a failure, the failure is propagated to the returned result.
    /// </summary>
    /// <param name="resultTask">The task containing the source result.</param>
    /// <param name="binder">An asynchronous function that returns a new non-generic result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the binder function when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static async Task<Result> BindAsync(this Task<Result> resultTask, Func<Task<Result>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(binder).ConfigureAwait(false);
    }

    #endregion

    #region EnsureAsync

    /// <summary>
    /// Asynchronously validates the value of a successful <see cref="Result{T}"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">An asynchronous validation function that returns true if the value is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the original <see cref="Result{T}"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result{T}"/> with the provided error.
    /// </returns>
    public static async Task<Result<T>> EnsureAsync<T>(
        this Result<T> result,
        Func<T, Task<bool>> predicate,
        Error error)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        var isValid = await predicate(result.Value).ConfigureAwait(false);
        return isValid ? result : Result.Failure<T>(error);
    }

    /// <summary>
    /// Validates the value of a successful result from an asynchronous <see cref="Task{Result}"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="resultTask">The task containing the result to validate.</param>
    /// <param name="predicate">A synchronous validation function that returns true if the value is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the original <see cref="Result{T}"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result{T}"/> with the provided error.
    /// </returns>
    public static async Task<Result<T>> EnsureAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, bool> predicate,
        Error error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Ensure(predicate, error);
    }

    /// <summary>
    /// Asynchronously validates the value of a successful result from an asynchronous <see cref="Task{Result}"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="resultTask">The task containing the result to validate.</param>
    /// <param name="predicate">An asynchronous validation function that returns true if the value is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the original <see cref="Result{T}"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result{T}"/> with the provided error.
    /// </returns>
    public static async Task<Result<T>> EnsureAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, Task<bool>> predicate,
        Error error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.EnsureAsync(predicate, error).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously validates a successful non-generic <see cref="Result"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">An asynchronous validation function that returns true if the result is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the original <see cref="Result"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result"/> with the provided error.
    /// </returns>
    public static async Task<Result> EnsureAsync(
        this Result result,
        Func<Task<bool>> predicate,
        Error error)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        var isValid = await predicate().ConfigureAwait(false);
        return isValid ? result : Result.Failure(error);
    }

    /// <summary>
    /// Validates a successful non-generic result from an asynchronous <see cref="Task{Result}"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <param name="resultTask">The task containing the result to validate.</param>
    /// <param name="predicate">A synchronous validation function that returns true if the result is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the original <see cref="Result"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result"/> with the provided error.
    /// </returns>
    public static async Task<Result> EnsureAsync(
        this Task<Result> resultTask,
        Func<bool> predicate,
        Error error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Ensure(predicate, error);
    }

    /// <summary>
    /// Asynchronously validates a successful non-generic result from an asynchronous <see cref="Task{Result}"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <param name="resultTask">The task containing the result to validate.</param>
    /// <param name="predicate">An asynchronous validation function that returns true if the result is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the original <see cref="Result"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result"/> with the provided error.
    /// </returns>
    public static async Task<Result> EnsureAsync(
        this Task<Result> resultTask,
        Func<Task<bool>> predicate,
        Error error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.EnsureAsync(predicate, error).ConfigureAwait(false);
    }

    #endregion
}