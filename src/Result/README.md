# Kubis1982.Result

A lightweight and fluent implementation of the Result pattern for .NET, providing type-safe error handling without exceptions.

## Features

- Type-safe error handling with Result and Result<T> types
- Fluent API for chaining operations
- Railway-oriented programming pattern support
- Minimal dependencies - pure .NET implementation
- Rich ResultError structure with codes and descriptions

## Installation

```bash
dotnet add package Kubis1982.Result
```

## Quick Start

### Creating Results

```csharp

// Success without value

var success = Result.Success(); // Result

// Success with value

var successWithValue = Result.Success(42); // Result<int>
Result<int> successWithValue2 = 42;

// Failure

var error = new ResultError("NOT_FOUND", "User not found"); // or ResultError.Custom("NOT_FOUND", "User not found")
var failure = Result.Failure(error);

Result<int> failureWithValue = ResultError.Validation("Invalid input"); // or other predefined error types

```

## Core Types

- **Result**: Non-generic result for operations without return values
- **Result<T>**: Generic result for operations returning values on success
- **ResultError**: Structured error information with code and description
- **ResultException**: Exception type for exceptional scenarios

## Related Packages

- **Kubis1982.Result.AspNet**: ASP.NET Core integration with IResult and ProblemDetails
- **Kubis1982.Result.WolverineFx**: WolverineFx message handling integration

## License

MIT License - see [LICENCE](../../LICENCE) file

## Author

Mariusz Świątnicki  
https://github.com/kubis1982/Result
