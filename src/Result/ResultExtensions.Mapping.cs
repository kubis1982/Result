namespace Kubis1982.Result;

public static partial class ResultExtensions
{
    #region Mapping

    /// <summary>
    /// Projects the value of a successful <see cref="Result{T}"/> to a new value of type <typeparamref name="TOut"/>.
    /// If the current result is a failure, the failure is propagated to the returned <see cref="Result{TOut}"/>.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="mapper">A mapping function applied to the inner value when the result is successful.</param>
    /// <returns>
    /// A <see cref="Result{TOut}"/> containing the mapped value when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static Result<TOut> Map<T, TOut>(this Result<T> result, Func<T, TOut> mapper)
    {
        if (!result.IsSuccess)
        {
            return Result.Failure<TOut>(result.Error);
        }

        return Result<TOut>.Success(mapper(result.Value));
    }

    /// <summary>
    /// Converts a <see cref="Result{TSource}"/> to a non-generic <see cref="Result"/>.
    /// If the current result is a failure, the failure is propagated to the returned <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="TSource">The source value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>
    /// A non-generic <see cref="Result"/> with success status when this result is successful;
    /// otherwise a failed <see cref="Result"/> with the same error.
    /// </returns>
    public static Result Map<TSource>(this Result<TSource> result)
    {
        if (!result.IsSuccess)
        {
            return Result.Failure(result.Error);
        }

        return Result.Success();
    }

    /// <summary>
    /// Maps a non-generic <see cref="Result"/> to <see cref="Result{TOut}"/>.
    /// If the current result is a failure, the failure is propagated to the returned <see cref="Result{TOut}"/>.
    /// If the current result is successful, returns a successful <see cref="Result{TOut}"/> containing the default value of <typeparamref name="TOut"/>.
    /// </summary>
    /// <typeparam name="TOut">The target value type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <returns>
    /// A <see cref="Result{TOut}"/> containing the default value when this result is successful;
    /// otherwise a failed <see cref="Result{TOut}"/> with the same error.
    /// </returns>
    public static Result<TOut> Map<TOut>(this Result result)
    {
        if (!result.IsSuccess)
        {
            return Result.Failure<TOut>(result.Error);
        }

        return Result.Success<TOut>(default!);
    }

    #endregion
}
