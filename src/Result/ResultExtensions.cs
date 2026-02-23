namespace Kubis1982.Result;

public static class ResultExtensions
{
    #region Mapping

    /// <summary>
    /// Projects the value of a successful <see cref="Result{T}"/> to a new value of type <typeparamref name="TOut"/>.
    /// If the current result is a failure, the failure is propagated to the returned <see cref="Result{TOut}"/>.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="result"></param>
    /// <param name="mapper">A mapping function applied to the inner value when the result is successful.</param>
    /// <returns>
    /// A <see cref="Result{TOut}"/> containing the mapped value when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static Result<TOut> Map<T, TOut>(this Result<T> result, Func<T, TOut> mapper)
    {
        if (!result.IsSuccess)
        {
            return Result<TOut>.Failure(result.Error!.Value);
        }

        return Result<TOut>.Success(mapper(result.Value));
    }

    /// <summary>
    /// Attempts to map this result to <see cref="Result{TOut}"/> without a mapping function.
    /// This overload only propagates failures — when this result is successful, calling this method
    /// is considered an invalid operation and an <see cref="InvalidOperationException"/> is thrown.
    /// </summary>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <returns>
    /// A failed <see cref="Result{TOut}"/> containing the same error if this result is a failure.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when this result is successful because no mapper was provided.</exception>
    public static Result<TOut> Map<TOut>(this Result result)
    {
        if (!result.IsSuccess)
        {
            return Result<TOut>.Failure(result.Error!.Value);
        }

        throw new InvalidOperationException("Cannot map success result without mapping function. Use Map<TOut>(Func<T, TOut> mapper) instead.");
    }

    #endregion
}