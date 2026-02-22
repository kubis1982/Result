# FluentResult

A lightweight, fluent .NET library for handling operation results with built-in error handling and ASP.NET Core integration.

## Features

- 🎯 **Fluent API** - Intuitive and readable result handling
- ⚡ **Lightweight** - Minimal dependencies and overhead
- 🔄 **Generic Support** - Type-safe results with `Result<T>`
- 🌐 **ASP.NET Core Integration** - Seamless conversion to `IResult`
- 📝 **Rich Error Information** - Structured error codes and descriptions
- 🛡️ **Type Safety** - Compile-time safety for result handling

## Installation

### Core Library
```bash
dotnet add package kubis1982.FluentResult
```

### ASP.NET Core Extensions
```bash
dotnet add package kubis1982.FluentResult.Extensions.AspNet
```

### Wolverine Extensions
```bash
dotnet add package kubis1982.FluentResult.Extensions.Wolverine
```

## Quick Start

### Basic Usage

```csharp
using kubis1982.FluentResult;

// Success result
Result successResult = Result.Success();

// Generic success result
Result<string> dataResult = Result.Success("Hello World");
// or using implicit conversion
Result<string> dataResult = "Hello World";

// Failure result
Result failureResult = Result.Failure(ResultError.NotFound("User not found"));
// or using implicit conversion
Result failureResult = ResultError.NotFound("User not found");
```

### Error Types

The library provides predefined error types with appropriate HTTP status code mappings:

```csharp
// Predefined error types
var notFound = ResultError.NotFound("Resource not found");
var conflict = ResultError.Conflict("Resource already exists");
var forbidden = ResultError.Forbidden("Access denied");
var unauthorized = ResultError.Unauthorized("Authentication required");
var validation = ResultError.Validation("Invalid input data");

// Custom error
var customError = new ResultError("custom.code", "Custom error description");
```

### Working with Generic Results

```csharp
public Result<User> GetUser(int id)
{
    if (id <= 0)
        return ResultError.Validation("Invalid user ID");

    var user = _userRepository.Find(id);
    if (user == null)
        return ResultError.NotFound($"User with ID {id} not found");

    return user; // Implicit conversion to Result<User>
}

// Usage
var result = GetUser(123);
if (result.IsSuccess)
{
    Console.WriteLine($"User: {result.Value.Name}");
}
else
{
    Console.WriteLine($"Error: {result.Error?.Description}");
}
```

### Result Mapping

Transform successful results while preserving failures:

```csharp
Result<User> userResult = GetUser(123);

// Map User to UserDto
Result<UserDto> dtoResult = userResult.Map(user => new UserDto 
{
    Id = user.Id,
    Name = user.Name
});
```

### Safe Value Access

```csharp
Result<string> result = GetSomeValue();

// Throws ResultException if failed
string value = result.Value;

// Safe access with default value
string safeValue = result.ValueOrDefault ?? "default";

// Explicit casting (throws if failed)
string explicitValue = (string)result;
```

## ASP.NET Core Integration

The ASP.NET Core extensions provide seamless integration with minimal APIs and controllers.

### Setup

Add the namespace to your `Program.cs` or controller:

```csharp
using kubis1982.FluentResult;
```

### Minimal APIs

```csharp
app.MapGet("/users/{id}", (int id, IUserService userService) =>
{
    return userService.GetUser(id).ToResult();
});

app.MapPost("/users", (CreateUserRequest request, IUserService userService) =>
{
    return userService.CreateUser(request).ToResult();
});
```

### Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public IResult GetUser(int id)
    {
        return _userService.GetUser(id).ToResult();
    }

    [HttpPost]
    public IResult CreateUser(CreateUserRequest request)
    {
        return _userService.CreateUser(request).ToResult();
    }
}
```

### HTTP Status Code Mapping

The library automatically maps error codes to appropriate HTTP status codes:

| Error Type | HTTP Status | Code |
|------------|------------|------|
| `NotFound` | 404 Not Found | `general.notfound` |
| `Conflict` | 409 Conflict | `general.conflict` |
| `Forbidden` | 403 Forbidden | `general.forbidden` |
| `Unauthorized` | 401 Unauthorized | `general.unauthorized` |
| `Validation` | 422 Unprocessable Entity | `general.validation` |
| Custom/Other | 400 Bad Request | *custom* |

## Advanced Usage

### Service Implementation Example

```csharp
public interface IUserService
{
    Result<User> GetUser(int id);
    Result<User> CreateUser(CreateUserRequest request);
    Result UpdateUser(int id, UpdateUserRequest request);
    Result DeleteUser(int id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public Result<User> GetUser(int id)
    {
        if (id <= 0)
            return ResultError.Validation("User ID must be positive");

        var user = _repository.GetById(id);
        return user != null 
            ? Result.Success(user)
            : ResultError.NotFound($"User with ID {id} not found");
    }

    public Result<User> CreateUser(CreateUserRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email))
            return ResultError.Validation("Email is required");

        // Check for conflicts
        if (_repository.ExistsByEmail(request.Email))
            return ResultError.Conflict($"User with email {request.Email} already exists");

        // Create user
        var user = new User 
        { 
            Email = request.Email, 
            Name = request.Name 
        };

        _repository.Add(user);
        return user;
    }

    public Result UpdateUser(int id, UpdateUserRequest request)
    {
        var user = _repository.GetById(id);
        if (user == null)
            return ResultError.NotFound($"User with ID {id} not found");

        user.Name = request.Name;
        user.Email = request.Email;

        _repository.Update(user);
        return Result.Success();
    }

    public Result DeleteUser(int id)
    {
        var user = _repository.GetById(id);
        if (user == null)
            return ResultError.NotFound($"User with ID {id} not found");

        _repository.Delete(user);
        return Result.Success();
    }
}
```

### Error Response Format

When converted to `IResult` in ASP.NET Core, errors return a standard problem details response:

```json
{
  "type": "general.validation",
  "title": "Email is required",
  "detail": "Email is required",
  "status": 422
}
```

## Best Practices

1. **Use implicit conversions** for cleaner code:
   ```csharp
   // Instead of Result.Success(user)
   return user;

   // Instead of Result.Failure(error)
   return ResultError.NotFound("Not found");
   ```

2. **Prefer specific error types** over generic ones:
   ```csharp
   // Good
   return ResultError.Validation("Invalid email format");

   // Less descriptive
   return new ResultError("error", "Something went wrong");
   ```

3. **Use mapping** to transform between different result types:
   ```csharp
   return userResult.Map(user => user.ToDto());
   ```

4. **Handle both success and failure cases**:
   ```csharp
   var result = GetUser(id);
   return result.IsSuccess 
       ? result.ToResult()
       : result.ToResult(); // Both paths handled
   ```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.