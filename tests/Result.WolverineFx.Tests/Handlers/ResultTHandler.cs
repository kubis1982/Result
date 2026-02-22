namespace kubis1982.Result.Handlers
{
    public static class ResultTHandler
    {
        public static Task<Result<Contractor>> LoadAsync(ResultTCommand command)
        {
            var contractor = Repository.Contractors.FirstOrDefault(u => u.Id == command.ContractorId);

            if (contractor == null) return Task.FromResult((Result<Contractor>)ResultError.NotFound("Contractor not found"));

            return Task.FromResult(Result.Success(contractor));
        }

        public static Task<Result<ContractorDto>> Handle(ResultTCommand command, Contractor contractor)
        {
            var contractorDto = new ContractorDto(contractor.Id, contractor.Name);
            return Task.FromResult(Result.Success(contractorDto));
        }
    }
}
