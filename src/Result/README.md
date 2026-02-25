# Kubis1982.Result

A lightweight and fluent implementation of the Result pattern for .NET, providing type-safe error handling without exceptions.

## Features

- Type-safe error handling with Result and Result<T> types
- Fluent API for chaining operations
- Railway-oriented programming pattern support
- Minimal dependencies - pure .NET implementation
- Rich Error structure with codes and descriptions

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
Result<int> successWithValue2 = 42; // Implicit conversion

// Failure - using factory methods (recommended)
var notFoundResult = Result.NotFound("User not found");
var validationResult = Result.Validation("Invalid email format");
var unauthorizedResult = Result.Unauthorized("Access denied");

// Failure - with custom error code
var customError = Result.NotFound("USER.NOT_FOUND", "User with ID 123 not found");

// Generic failure
Result<int> failureWithValue = Result.Failure<int>(Error.Validation("Invalid input"));
Result<int> typedFailure = Result.NotFound<int>("Resource not found");

// Implicit conversion from Error
Result<int> implicitFailure = Error.Validation("Invalid input");
```

## Core Types

- **Result**: Non-generic result for operations without return values
- **Result<T>**: Generic result for operations returning values on success
- **Error**: Structured error information with code and description
- **ResultException**: Exception type for exceptional scenarios

## Core Properties

### IsSuccess / IsFailure

Every `Result` has two complementary properties:

```csharp
var successResult = Result.Success();
Console.WriteLine(successResult.IsSuccess);  // true
Console.WriteLine(successResult.IsFailure);  // false

var failureResult = Result.NotFound("User not found");
Console.WriteLine(failureResult.IsSuccess);  // false
Console.WriteLine(failureResult.IsFailure);  // true

// Use in conditional logic (prefer IsFailure for error checks)
if (result.IsFailure)
{
    // Handle error
    Console.WriteLine($"Error: {result.Error.Description}");
    return result;
}

// Early return pattern
public Result<User> GetUser(int id)
{
    var validationResult = ValidateId(id);
    if (validationResult.IsFailure)
        return validationResult.Error;

    // Continue processing...
}
```

### TryGetValue

Safely retrieve the value without throwing exceptions:

```csharp
var result = GetUser(userId);

// Pattern matching style
if (result.TryGetValue(out var user))
{
    Console.WriteLine($"User: {user.Name}");
}
else
{
    Console.WriteLine($"Error: {result.Error.Description}");
}

// Inline usage
var name = result.TryGetValue(out var value) ? value.Name : "Unknown";

// No exception thrown on failure
var failedResult = Result.NotFound<int>("Not found");
failedResult.TryGetValue(out var val); // Returns false, val is default(int)
```

## Extension Methods

### Mapping

Transform successful results to new values:

```csharp
// Simple mapping chain
var result = Result.Success(42)
    .Map(x => x * 2)           // Result<int> with value 84
    .Map(x => x.ToString());   // Result<string> with value "84"

// Mapping preserves errors
var failedResult = Result.NotFound<int>("Not found")
    .Map(x => x * 2)           // Skips mapping
    .Map(x => x.ToString());   // Still a failure with original error

// Convert to non-generic Result
var nonGeneric = Result.Success(42).Map(); // Result (discards value)

// Real-world example
public Result<UserDto> GetUserDto(int userId)
{
    return GetUser(userId)           // Result<User>
        .Map(user => new UserDto     // Result<UserDto>
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email
        });
}
```

### Binding (Chaining)

Chain operations that return Result types:

```csharp
// Basic binding
var result = Result.Success(10)
    .Bind(x => ValidatePositive(x))    // Returns Result<int>
    .Bind(x => SaveToDatabase(x));     // Returns Result<int>

// Real-world example: User registration
public Result<User> RegisterUser(string email, string password)
{
    return ValidateEmail(email)
        .Bind(() => ValidatePassword(password))
        .Bind(() => CheckEmailNotExists(email))
        .Bind(() => CreateUser(email, password))
        .Bind(user => SendWelcomeEmail(user))
        .Map(user => user); // Returns Result<User>
}

// Mixing Map and Bind
public Result<OrderDto> CreateOrder(CreateOrderRequest request)
{
    return ValidateRequest(request)              // Result
        .Bind(() => CalculateTotal(request))     // Result<decimal>
        .Bind(total => CreateOrderEntity(request, total))  // Result<Order>
        .Map(order => MapToDto(order));          // Result<OrderDto>
}
```

### Combine (Fail-Fast)

Combine multiple results into a single result. Returns the first failure encountered:

```csharp
// Non-generic: Validate multiple conditions
var nameValidation = ValidateName(name);
var emailValidation = ValidateEmail(email);
var ageValidation = ValidateAge(age);

var validationResult = ResultExtensions.Combine(
    nameValidation, 
    emailValidation, 
    ageValidation
);

if (validationResult.IsFailure)
{
    // At least one validation failed
    return validationResult;
}

// Generic: Collect multiple values
var result1 = GetValue1();  // Result<int>
var result2 = GetValue2();  // Result<int>
var result3 = GetValue3();  // Result<int>

var combined = ResultExtensions.Combine(result1, result2, result3);
// Returns Result<IEnumerable<int>> with all values if successful
// Returns first failure if any result failed

if (combined.IsSuccess)
{
    var sum = combined.Value.Sum();
    Console.WriteLine($"Total: {sum}");
}
```

**Use Cases for Combine:**
- Form validation (combine multiple field validations)
- Multi-step business rule validation
- Batch operations (all must succeed)
- Dependency checks before proceeding

**Important:** `Combine` implements fail-fast behavior - it stops at the first failure and returns immediately without evaluating remaining results.

## Common Patterns

### Early Return Pattern

```csharp
public Result<Invoice> GenerateInvoice(int orderId)
{
    var orderResult = GetOrder(orderId);
    if (orderResult.IsFailure)
        return orderResult.Error;

    var customerResult = GetCustomer(orderResult.Value.CustomerId);
    if (customerResult.IsFailure)
        return customerResult.Error;

    return CreateInvoice(orderResult.Value, customerResult.Value);
}
```

### Railway-Oriented Programming

```csharp
public Result<OrderConfirmation> ProcessOrder(OrderRequest request)
{
    return ValidateOrder(request)
        .Bind(order => CheckInventory(order))
        .Bind(order => ReserveItems(order))
        .Bind(order => ProcessPayment(order))
        .Bind(order => CreateShipment(order))
        .Map(order => new OrderConfirmation(order.Id));
}
```

### Conditional Processing

```csharp
public Result<User> UpdateUser(int userId, UpdateUserRequest request)
{
    return GetUser(userId)
        .Bind(user => ValidateUpdatePermissions(user))
        .Bind(user => request.Email != null 
            ? UpdateEmail(user, request.Email)
            : Result.Success(user))
        .Bind(user => request.Name != null
            ? UpdateName(user, request.Name)
            : Result.Success(user))
        .Bind(user => SaveUser(user));
}
```

### Error Type Patterns

```csharp
// Use factory methods for common error types
public Result<User> AuthenticateUser(string username, string password)
{
    var user = _repository.FindByUsername(username);
    if (user is null)
        return Result.NotFound("USER.NOT_FOUND", $"User '{username}' not found");

    if (!_passwordHasher.Verify(password, user.PasswordHash))
        return Result.Unauthorized("INVALID.CREDENTIALS", "Invalid username or password");

    if (!user.IsActive)
        return Result.Forbidden("USER.INACTIVE", "User account is inactive");

    if (!user.EmailVerified)
        return Result.Validation("EMAIL.NOT_VERIFIED", "Please verify your email first");

    return Result.Success(user);
}
```

## Related Packages

- **Kubis1982.Result.AspNet**: ASP.NET Core integration with IResult and ProblemDetails
- **Kubis1982.Result.WolverineFx**: WolverineFx message handling integration

## License

MIT License - see [LICENCE](../../LICENCE) file

## Author

Mariusz Świątnicki  
https://github.com/kubis1982/Result
