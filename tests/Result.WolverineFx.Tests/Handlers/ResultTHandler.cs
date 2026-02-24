namespace Kubis1982.Result.Handlers
{
    using Wolverine;

    public static class ResultTHandler
    {
        public static Task<Result<Contractor>> LoadAsync(ResultTCommand command)
        {
            var contractor = Repository.Contractors.FirstOrDefault(u => u.Id == command.ContractorId);

            if (contractor == null) return Task.FromResult((Result<Contractor>)Error.NotFound("Contractor not found"));

            return Task.FromResult(Result.Success(contractor));
        }

        public static Task<Result<ContractorDto>> Handle(ResultTCommand command, Contractor contractor)
        {
            var contractorDto = new ContractorDto(contractor.Id, contractor.Name);
            return Task.FromResult(Result.Success(contractorDto));
        }

        public static Task<Result<ContractorDto>> InvokeAsync(this ResultTCommand command, IMessageBus messageBus, CancellationToken cancellationToken)
        {
            return messageBus.InvokeAsync<Result<ContractorDto>>(command, cancellationToken);
        }
    }
}
