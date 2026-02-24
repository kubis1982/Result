namespace Kubis1982.Result;

/// <summary>
/// Exception thrown when accessing the value of a failed <see cref="Result"/> or <see cref="Result{T}"/>.
/// Contains the originating <see cref="Kubis1982.Result.Error"/>.
/// </summary>
/// <param name="error">The error that caused the exception.</param>
public class ResultException(Error error) : Exception($"Result is in failure state. Error: {error.Code} - {error.Description}")
{
    /// <summary>
    /// The <see cref="Kubis1982.Result.Error"/> associated with the failure.
    /// </summary>
    public Error Error { get; } = error;
}
