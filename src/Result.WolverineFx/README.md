# Kubis1982.Result.WolverineFx

WolverineFx extensions for the Result pattern, providing message handling and continuation strategies.

## Features

- Seamless integration with WolverineFx message handlers
- Automatic continuation strategies for Result types
- Support for both Result and Result<T> return types
- Type-safe message handling with railway-oriented programming

## Installation

```bash
dotnet add package Kubis1982.Result.WolverineFx
```

## Quick Start

### Message Handler with Result<T>

```csharp
public record ResultTCommand(int ContractorId);

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

    public static Task<Result<ContractorDto>> InvokeAsync(this ResultTCommand command, IMessageBus messageBus, CancellationToken cancellationToken)
    {
        return messageBus.InvokeAsync<Result<ContractorDto>>(command, cancellationToken);
    }
}

var command = new ResultTCommand(Repository.CONTRACTOR_ID_JOHN_DOE);

var result = await command.InvokeAsync(bus, TestContext.Current.CancellationToken);

```

## How It Works

The library provides ResultContinuationStrategy that automatically handles Result return types:

- On Success: Continues with the message pipeline
- On Failure: Stops the pipeline and handles the error appropriately

## Configuration

Register the continuation strategy in your WolverineFx configuration:

```csharp
using Wolverine.Middleware;

builder.Host.UseWolverine(options =>
{
    options.CodeGeneration.AddContinuationStrategy<ResultContinuationStrategy>();
});
```

## Benefits

- Clean separation of business logic and infrastructure concerns
- Type-safe error handling in message handlers
- Automatic pipeline control based on operation results
- Railway-oriented programming in distributed systems

## Related Packages

- Kubis1982.Result - Core library
- Kubis1982.Result.AspNet - ASP.NET Core integration

## License

MIT License - see LICENCE file

## Author

Kubis1982
https://github.com/kubis1982/Result
