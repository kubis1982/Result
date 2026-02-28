namespace Kubis1982.Result;

public static partial class ResultExtensions
{
    #region Ensure

    /// <summary>
    /// Validates the value of a successful <see cref="Result{T}"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">A validation function that returns true if the value is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// The original <see cref="Result{T}"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result{T}"/> with the provided error.
    /// </returns>
    /// <example>
    /// <code>
    /// return GetUser(id)
    ///     .Ensure(user => user.IsActive, Error.Validation("User is not active"))
    ///     .Ensure(user => !user.IsDeleted, Error.NotFound("User was deleted"));
    /// </code>
    /// </example>
    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        Error error)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        return predicate(result.Value) ? result : Result.Failure<T>(error);
    }

    /// <summary>
    /// Validates a successful non-generic <see cref="Result"/> using a predicate.
    /// If the predicate returns false, the result is converted to a failure with the provided error.
    /// If the current result is already a failure, the failure is propagated unchanged.
    /// </summary>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">A validation function that returns true if the result is valid.</param>
    /// <param name="error">The error to return if the validation fails.</param>
    /// <returns>
    /// The original <see cref="Result"/> if it is successful and the predicate returns true;
    /// otherwise a failed <see cref="Result"/> with the provided error.
    /// </returns>
    public static Result Ensure(
        this Result result,
        Func<bool> predicate,
        Error error)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        return predicate() ? result : Result.Failure(error);
    }

    #endregion
}
