namespace Kubis1982.Result;

/// <summary>
/// Represents the result of an operation that produces a value of type <typeparamref name="T"/> on success.
/// Inherits from <see cref="Result"/>.
/// </summary>
public sealed partial class Result<T> : Result
{
    private readonly T? _value = default;

    internal Result(Error error) : base(error)
    {
    }

    internal Result(T value) : base()
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value when the result is successful. Throws <see cref="ResultException"/> when the result is a failure.
    /// </summary>
    public T Value
    {
        get
        {
            if (!IsSuccess)
            {
                throw new ResultException(Error);
            }

            return _value!;
        }
    }

    /// <summary>
    /// Attempts to get the value from the result.
    /// Returns true if the result is successful and sets the value parameter, otherwise returns false.
    /// </summary>
    /// <param name="value">When this method returns, contains the value if the result is successful, or the default value of <typeparamref name="T"/> if the result is a failure.</param>
    /// <returns>True if the result is successful; otherwise, false.</returns>
    public bool TryGetValue(out T? value)
    {
        if (IsSuccess)
        {
            value = _value;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Deconstructs the result into its success state, value, and error.
    /// </summary>
    /// <param name="isSuccess">True if the result is successful; otherwise, false.</param>
    /// <param name="value">The value if the result is successful; otherwise, the default value of <typeparamref name="T"/>.</param>
    /// <param name="error">The error if the result is a failure; otherwise, an error with <see cref="ErrorType.None"/>.</param>
    public void Deconstruct(out bool isSuccess, out T? value, out Error error)
    {
        isSuccess = IsSuccess;
        value = IsSuccess ? _value : default;
        error = Error;
    }
}

partial class Result<T>
{
    #region Operators

    /// <summary>
    /// Implicitly converts a value of <typeparamref name="T"/> into a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a successful result.</param>
    public static implicit operator Result<T>(T value) => new(value);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> into a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error to convert into a failed result.</param>
    public static implicit operator Result<T>(Error error) => new(error);

    /// <summary>
    /// Explicitly extracts the value from a successful <see cref="Result{T}"/>.
    /// Throws <see cref="ResultException"/> if the result is a failure.
    /// </summary>
    /// <param name="result">The result to extract the value from.</param>
    public static explicit operator T(Result<T> result) => result.Value;

    #endregion
}

