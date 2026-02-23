namespace Kubis1982.Result.Handlers
{
    using Wolverine;

    public static class ResultHandler
    {
        public const string ContractorNotFoundMessage = "Contractor not found";

        public static async Task<Result> LoadAsync(ResultCommand command)
        {
            var contractor = Repository.Contractors.FirstOrDefault(u => u.Id == command.ContractorId);

            if (contractor == null) return ResultError.NotFound(ContractorNotFoundMessage);

            return Result.Success();
        }

        public static Task<Result<string>> Handle(ResultCommand command)
        {
            return Task.FromResult(Result.Success($"Processed command with ID: {command.ContractorId}"));
        }

        public static Task<Result<string>> InvokeAsync(this ResultCommand command, IMessageBus messageBus, CancellationToken cancellationToken)
        {
            return messageBus.InvokeAsync<Result<string>>(command, cancellationToken);
        }
    }
}
