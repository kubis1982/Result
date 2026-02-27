namespace Kubis1982.Result;

/// <summary>
/// Asynchronous extension methods for Match pattern on Result types.
/// </summary>
public static partial class ResultExtensions
{
    #region MatchAsync - Non-Generic Result

    /// <summary>
    /// Asynchronously matches the non-generic result to one of two functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The asynchronous function to execute when the result is successful.</param>
    /// <param name="onFailure">The asynchronous function to execute when the result is a failure, receiving the error.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the value returned by either onSuccess or onFailure function.
    /// </returns>
    public static async Task<TOut> MatchAsync<TOut>(
        this Result result,
        Func<Task<TOut>> onSuccess,
        Func<Error, Task<TOut>> onFailure)
    {
        return result.IsSuccess
            ? await onSuccess().ConfigureAwait(false)
            : await onFailure(result.Error).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously matches the non-generic result from a task to one of two functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="resultTask">The task containing the result to match.</param>
    /// <param name="onSuccess">The synchronous function to execute when the result is successful.</param>
    /// <param name="onFailure">The synchronous function to execute when the result is a failure, receiving the error.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the value returned by either onSuccess or onFailure function.
    /// </returns>
    public static async Task<TOut> MatchAsync<TOut>(
        this Task<Result> resultTask,
        Func<TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Asynchronously matches the non-generic result from a task to one of two asynchronous functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="resultTask">The task containing the result to match.</param>
    /// <param name="onSuccess">The asynchronous function to execute when the result is successful.</param>
    /// <param name="onFailure">The asynchronous function to execute when the result is a failure, receiving the error.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the value returned by either onSuccess or onFailure function.
    /// </returns>
    public static async Task<TOut> MatchAsync<TOut>(
        this Task<Result> resultTask,
        Func<Task<TOut>> onSuccess,
        Func<Error, Task<TOut>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MatchAsync(onSuccess, onFailure).ConfigureAwait(false);
    }

    #endregion

    #region MatchAsync - Generic Result

    /// <summary>
    /// Asynchronously matches the generic result to one of two functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The asynchronous function to execute when the result is successful, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to execute when the result is a failure, receiving the error.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the value returned by either onSuccess or onFailure function.
    /// </returns>
    public static async Task<TOut> MatchAsync<T, TOut>(
        this Result<T> result,
        Func<T, Task<TOut>> onSuccess,
        Func<Error, Task<TOut>> onFailure)
    {
        return result.IsSuccess
            ? await onSuccess(result.Value).ConfigureAwait(false)
            : await onFailure(result.Error).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously matches the generic result from a task to one of two functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="resultTask">The task containing the result to match.</param>
    /// <param name="onSuccess">The synchronous function to execute when the result is successful, receiving the value.</param>
    /// <param name="onFailure">The synchronous function to execute when the result is a failure, receiving the error.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the value returned by either onSuccess or onFailure function.
    /// </returns>
    public static async Task<TOut> MatchAsync<T, TOut>(
        this Task<Result<T>> resultTask,
        Func<T, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Asynchronously matches the generic result from a task to one of two asynchronous functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="resultTask">The task containing the result to match.</param>
    /// <param name="onSuccess">The asynchronous function to execute when the result is successful, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to execute when the result is a failure, receiving the error.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the value returned by either onSuccess or onFailure function.
    /// </returns>
    public static async Task<TOut> MatchAsync<T, TOut>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TOut>> onSuccess,
        Func<Error, Task<TOut>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MatchAsync(onSuccess, onFailure).ConfigureAwait(false);
    }

    #endregion
}
