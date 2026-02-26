namespace Kubis1982.Result.Handlers
{
    using Wolverine;

    public static class ResultHandler
    {
        public const string ContractorNotFoundMessage = "Contractor not found";

        public static Task<Result> ValidateAsync(ResultCommand command)
        {
            return Task.FromResult(Result.Success());
        }

        public static Task<Result> BeforeAsync(ResultCommand command)
        {
            return Task.FromResult(Result.Success());
        }

        public static async Task<Result<Contractor>> LoadAsync(ResultCommand command)
        {
            var contractor = Repository.Contractors.FirstOrDefault(u => u.Id == command.ContractorId);

            if (contractor == null) return Error.NotFound(ContractorNotFoundMessage);

            return Result.Success(contractor);
        }

        public static Task<Result<string>> Handle(ResultCommand command, Contractor contractor)
        {
            return Task.FromResult(Result.Success($"Processed command with ID: {command.ContractorId}"));
        }

        public static Task<Result<string>> InvokeAsync(this ResultCommand command, IMessageBus messageBus, CancellationToken cancellationToken)
        {
            return messageBus.InvokeAsync<Result<string>>(command, cancellationToken);
        }
    }
}
