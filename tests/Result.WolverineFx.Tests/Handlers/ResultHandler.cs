namespace kubis1982.Result.Handlers
{
    public static class ResultHandler
    {
        public static async Task<Result> LoadAsync(ResultCommand command)
        {
            await Task.CompletedTask;

            if (command.Id < 0)  return ResultError.Validation("ID cannot be negative");

            if (command.Id == 999) return ResultError.NotFound("Entity not found");

            return Result.Success();
        }

        public static Task<Result<string>> Handle(ResultCommand command)
        {
            return Task.FromResult(Result.Success($"Processed command with ID: {command.Id}"));
        }
    }
}
