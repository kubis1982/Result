namespace kubis1982.Result;

/// <summary>
/// Exception thrown when accessing the value of a failed <see cref="Result"/> or <see cref="Result{T}"/>.
/// Contains the originating <see cref="ResultError"/>.
/// </summary>
/// <param name="error">The error that caused the exception.</param>
public class ResultException(ResultError error) : Exception($"Result is in failure state. Error: {error.Code} - {error.Description}")
{
    /// <summary>
    /// The <see cref="ResultError"/> associated with the failure.
    /// </summary>
    public ResultError Error { get; } = error;
}
