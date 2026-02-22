namespace FluentResult;

public partial class Result<T> : Result
{
    private readonly T? _value;

    Result(bool isSuccess, T? value, ResultError? error = null) 
        : base(isSuccess, error)
    {
        _value = value;
    }

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

    public T? ValueOrDefault => IsSuccess ? _value : default;    
}


partial class Result<T>
{
    #region Factory Methods

    public static Result<T> Success(T value) => new(true, value);

    public new static Result<T> Failure(ResultError error) => new(false, default, error);

    #endregion

    #region Operators

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(ResultError error) => Failure(error);

    public static explicit operator T(Result<T> result) => result.Value;

    #endregion
}

