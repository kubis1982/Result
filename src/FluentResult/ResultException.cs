namespace FluentResult;

public class ResultException(ResultError error) : Exception($"Result is in failure state. Error: {error.Code} - {error.Description}")
{
    public ResultError Error { get; } = error;
}
