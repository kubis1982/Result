# Kubis1982.Result.AspNet

ASP.NET Core extensions for the Result pattern, providing seamless integration with ASP.NET Core IResult and ProblemDetails.

## Features

- Convert Result and Result<T> to ASP.NET Core IResult
- Automatic mapping of errors to HTTP status codes
- Integration with ProblemDetails (RFC 7807)
- Support for Minimal APIs and Controllers

## Installation

```bash
dotnet add package Kubis1982.Result.AspNet
```

## Quick Start

### Minimal API

app.MapGet("/users/id", (int id) => userService.GetUser(id).ToResult());

## Extension Methods

- ToResult() - Converts Result to IResult (204 No Content on success)
- ToResult<T>() - Converts Result<T> to IResult (200 OK with value on success)

## Error Mapping

- NOT_FOUND -> 404
- VALIDATION_ERROR -> 422
- UNAUTHORIZED -> 401
- FORBIDDEN -> 403
- CONFLICT -> 409

## Related Packages

- Kubis1982.Result - Core library
- Kubis1982.Result.WolverineFx - WolverineFx integration

## License

MIT License - see LICENCE file

## Author

Kubis1982
https://github.com/kubis1982/Result
