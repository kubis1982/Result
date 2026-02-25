namespace Kubis1982.Result;

public static partial class ResultExtensions
{
    #region Combine

    /// <summary>
    /// Combines multiple <see cref="Result"/> instances into a single <see cref="Result"/>.
    /// Returns the first failure encountered (fail-fast), or success if all results are successful.
    /// </summary>
    /// <param name="results">The results to combine.</param>
    /// <returns>
    /// A successful <see cref="Result"/> if all results are successful;
    /// otherwise the first failed <see cref="Result"/> encountered.
    /// </returns>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.Success();
    }

    /// <summary>
    /// Combines multiple <see cref="Result"/> instances from an enumerable into a single <see cref="Result"/>.
    /// Returns the first failure encountered (fail-fast), or success if all results are successful.
    /// </summary>
    /// <param name="results">The enumerable of results to combine.</param>
    /// <returns>
    /// A successful <see cref="Result"/> if all results are successful;
    /// otherwise the first failed <see cref="Result"/> encountered.
    /// </returns>
    public static Result Combine(IEnumerable<Result> results)
    {
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.Success();
    }

    /// <summary>
    /// Combines multiple <see cref="Result{T}"/> instances into a single result containing a collection of all values.
    /// Returns the first failure encountered (fail-fast), or a collection of all values if all results are successful.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the results.</typeparam>
    /// <param name="results">The results to combine.</param>
    /// <returns>
    /// A successful result containing an <see cref="IEnumerable{T}"/> with all values if all results are successful;
    /// otherwise the first failed result encountered.
    /// </returns>
    public static Result<IEnumerable<T>> Combine<T>(params Result<T>[] results)
    {
        var values = new List<T>(results.Length);

        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return Result.Failure<IEnumerable<T>>(result.Error);
            }

            values.Add(result.Value);
        }

        return Result.Success<IEnumerable<T>>(values);
    }

    /// <summary>
    /// Combines multiple <see cref="Result{T}"/> instances from an enumerable into a single result containing a collection of all values.
    /// Returns the first failure encountered (fail-fast), or a collection of all values if all results are successful.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the results.</typeparam>
    /// <param name="results">The enumerable of results to combine.</param>
    /// <returns>
    /// A successful result containing an <see cref="IEnumerable{T}"/> with all values if all results are successful;
    /// otherwise the first failed result encountered.
    /// </returns>
    public static Result<IEnumerable<T>> Combine<T>(IEnumerable<Result<T>> results)
    {
        var values = new List<T>();

        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return Result.Failure<IEnumerable<T>>(result.Error);
            }

            values.Add(result.Value);
        }

        return Result.Success<IEnumerable<T>>(values);
    }

    #endregion
}
