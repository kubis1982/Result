namespace kubis1982.Result;

/// <summary>
/// Represents the result of an operation that produces a value of type <typeparamref name="T"/> on success.
/// Inherits from <see cref="Result"/>.
/// </summary>
public partial class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// Internal constructor used by factory methods.
    /// </summary>
    internal Result(bool isSuccess, T? value, ResultError? error = null)
        : base(isSuccess, error)
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
                throw new ResultException(Error!.Value);
            }
            return _value!;
        }
    }

    /// <summary>
    /// Returns the contained value when successful, otherwise the default value for <typeparamref name="T"/>.
    /// </summary>
    public T? ValueOrDefault => IsSuccess ? _value : default;
}


partial class Result<T>
{
    #region Factory Methods

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> containing the provided value.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with the provided <see cref="ResultError"/>.
    /// </summary>
    public new static Result<T> Failure(ResultError error) => new(false, default, error);

    #endregion

    #region Operators

    /// <summary>
    /// Implicitly convert a value of <typeparamref name="T"/> into a successful <see cref="Result{T}"/>.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly convert a <see cref="ResultError"/> into a failed <see cref="Result{T}"/>.
    /// </summary>
    public static implicit operator Result<T>(ResultError error) => Failure(error);

    /// <summary>
    /// Explicitly extract the value from a successful <see cref="Result{T}"/>.
    /// Throws <see cref="ResultException"/> if the result is a failure.
    /// </summary>
    public static explicit operator T(Result<T> result) => result.Value;

    #endregion
}

