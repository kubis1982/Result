using Xunit;

namespace kubis1982.FluentResult.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        Result<int> result2 = 2;

        int result3 = (int)result2;

        var result4 = Result.Failure(ResultError.NotFound("Nie znaleziono elementu"));


        Result<int> d = ResultError.NotFound("Nie znaleziono elementu");

        Result e = ResultError.NotFound("Nie znaleziono elementu");


        Result f = new ResultError { Code = "NotFound", Description = "Nie znaleziono elementu" };
    }

    public Result<int> GetResult(int id, int e)
    {
        if (id == 1)
        {
            Result<string> s = ResultError.Validation("sdfsfsf");

            return s.Map<int>();
        }
        else
        {
            return ResultError.NotFound("Nie znaleziono elementu"); // Failure with error
        }

    }

    public Result GetResult2(int id, int e)
    {
        if (id == 1)
        {
            Result<string> s = ResultError.Validation("sdfsfsf");

            return s;
        }
        else
        {
            return ResultError.NotFound("Nie znaleziono elementu"); // Failure with error
        }

    }
}
