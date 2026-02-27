namespace Kubis1982.Result;

public static partial class ResultExtensions
{
    #region Match

    /// <summary>
    /// Matches the non-generic result to one of two functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The function to execute when the result is successful.</param>
    /// <param name="onFailure">The function to execute when the result is a failure, receiving the error.</param>
    /// <returns>The value returned by either onSuccess or onFailure function.</returns>
    public static TOut Match<TOut>(this Result result, Func<TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    /// <summary>
    /// Matches the generic result to one of two functions based on its success or failure state.
    /// This method enforces handling of both success and failure cases.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <typeparam name="TOut">The return type of both match functions.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The function to execute when the result is successful, receiving the value.</param>
    /// <param name="onFailure">The function to execute when the result is a failure, receiving the error.</param>
    /// <returns>The value returned by either onSuccess or onFailure function.</returns>
    public static TOut Match<T, TOut>(this Result<T> result, Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }

    #endregion
}
