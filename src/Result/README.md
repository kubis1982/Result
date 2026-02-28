# Kubis1982.Result

A lightweight and fluent implementation of the Result pattern for .NET, providing type-safe error handling without exceptions.

## Features

- Type-safe error handling with Result and Result<T> types
- Fluent API for chaining operations (Map, Bind, Ensure, Match)
- Railway-oriented programming pattern support
- Chainable validation with Ensure methods
- Full async/await support (MapAsync, BindAsync, EnsureAsync, MatchAsync)
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

### Deconstruct

Use tuple deconstruction to extract result components:

```csharp
// Non-generic Result
var result = ValidateInput(data);
var (isSuccess, error) = result;

if (isSuccess)
{
    Console.WriteLine("Validation passed");
}
else
{
    Console.WriteLine($"Validation failed: {error.Description}");
}

// Generic Result<T>
var userResult = GetUser(userId);
var (success, user, error) = userResult;

if (success)
{
    Console.WriteLine($"User: {user.Name}");
}
else
{
    Console.WriteLine($"Error: {error.Description}");
}

// Pattern matching with switch expressions
var message = userResult switch
{
    (true, var user, _) => $"Welcome, {user.Name}!",
    (false, _, var err) => $"Failed to load user: {err.Description}"
};

// Destructuring in method parameters
public void ProcessResult(Result<int> result)
{
    var (isSuccess, value, error) = result;

    if (isSuccess)
        Console.WriteLine($"Processing value: {value}");
    else
        Console.WriteLine($"Error: {error.Code}");
}
```

### Match (Pattern Matching)

Enforce exhaustive handling of both success and failure cases using extension methods:

```csharp
// Non-generic Result - returns value
var result = ValidateInput(data);
var message = result.Match(
    onSuccess: () => "Validation passed",
    onFailure: error => $"Validation failed: {error.Description}"
);

// Generic Result<T> - returns value based on inner value
var userResult = GetUser(userId);
var greeting = userResult.Match(
    onSuccess: user => $"Welcome, {user.Name}!",
    onFailure: error => $"Failed to load user: {error.Description}"
);

// Chaining with other operations
var result = GetUser(userId)
    .Map(user => user.Email)
    .Match(
        onSuccess: email => $"Email: {email}",
        onFailure: error => "No email available"
    );

// Real-world example: HTTP response mapping
public IActionResult GetUserEndpoint(int userId)
{
    return GetUser(userId).Match(
        onSuccess: user => Ok(user),
        onFailure: error => error.ErrorType switch
        {
            ErrorType.NotFound => NotFound(error),
            ErrorType.Unauthorized => Unauthorized(error),
            ErrorType.Validation => BadRequest(error),
            _ => StatusCode(500, error)
        }
    );
}

// Exhaustive handling - compile-time safety
var status = orderResult.Match(
    onSuccess: order => $"Order {order.Id} created",
    onFailure: error => $"Failed: {error.Code}"
    // Both cases MUST be handled - enforced by compiler
);

// Complex transformations
var response = result.Match(
    onSuccess: value => new { Success = true, Data = value, Error = (string?)null },
    onFailure: error => new { Success = false, Data = default(T), Error = error.Description }
);

// Integration with async operations
var finalResult = await GetDataAsync()
    .MapAsync(async data => await ProcessAsync(data))
    .BindAsync(async result => await SaveAsync(result))
    .MapAsync(async saved => saved.Id);

var message = finalResult.Match(
    onSuccess: id => $"Saved with ID: {id}",
    onFailure: error => $"Failed: {error.Description}"
);
```

**Why use Match?**
- **Exhaustive handling**: Compiler enforces handling of both cases
- **Expression-oriented**: Returns a value, perfect for functional style
- **Type-safe**: No nullable checks or exception handling needed
- **Readable**: Clear intent of handling success vs failure
- **Composable**: Works seamlessly with Map, Bind, and async operations
- **Extension method**: Consistent with the rest of the Result API

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

// Using extension method
var combined = ResultExtensions.Combine(result1, result2, result3);

// Or using static method (shorter)
var combined = Result.Combine(result1, result2, result3);

// Returns Result<IEnumerable<int>> with all values if successful
// Returns first failure if any result failed

if (combined.IsSuccess)
{
    var sum = combined.Value.Sum();
    Console.WriteLine($"Total: {sum}");
}

// Static method examples
// Non-generic
var validationResult = Result.Combine(
    ValidateName(name),
    ValidateEmail(email),
    ValidateAge(age)
);

// Generic
var userDataResult = Result.Combine(
    GetUserName(userId),      // Result<string>
    GetUserEmail(userId),     // Result<string>
    GetUserAge(userId)        // Result<int>
);
// This won't work as T must be the same type
// Use separate Combine calls or different approach

// Better: Combine same types
var numbersResult = Result.Combine(
    ParseNumber("10"),        // Result<int>
    ParseNumber("20"),        // Result<int>
    ParseNumber("30")         // Result<int>
);
```

**Use Cases for Combine:**
- Form validation (combine multiple field validations)
- Multi-step business rule validation
- Batch operations (all must succeed)
- Dependency checks before proceeding

**Important:** `Combine` implements fail-fast behavior - it stops at the first failure and returns immediately without evaluating remaining results.

### Ensure (Validation)

Validate results with chainable predicates, following Railway-Oriented Programming principles:

```csharp
// Single validation
var result = Result.Success(42)
    .Ensure(x => x > 0, Error.Validation("Value must be positive"));

// Chained validations - stops at first failure
var validated = Result.Success(user)
    .Ensure(u => u.IsActive, Error.Validation("User is not active"))
    .Ensure(u => u.Age >= 18, Error.Validation("User must be an adult"))
    .Ensure(u => !string.IsNullOrEmpty(u.Email), Error.Validation("Email is required"));

// Real-world example
public Result<User> GetActiveUser(int id)
{
    return GetUser(id)
        .Ensure(user => user.IsActive, Error.Validation("User is not active"))
        .Ensure(user => !user.IsDeleted, Error.NotFound("User was deleted"))
        .Ensure(user => user.EmailVerified, Error.Forbidden("Email not verified"));
}

// Non-generic Result validation
var result = Result.Success()
    .Ensure(() => ConfigurationIsValid(), Error.Configuration("Invalid configuration"))
    .Ensure(() => DatabaseIsConnected(), Error.Unavailable("Database unavailable"));

// Async validation with EnsureAsync
public async Task<Result<Order>> ProcessOrder(int orderId)
{
    return await GetOrderAsync(orderId)
        .Ensure(order => order.Items.Any(), Error.Validation("Order must contain items"))
        .EnsureAsync(
            async order => await HasSufficientStock(order.Items),
            Error.Conflict("Insufficient stock"))
        .EnsureAsync(
            async order => await IsCustomerEligible(order.CustomerId),
            Error.Forbidden("Customer not eligible"));
}

// Integration with other operations
public async Task<Result<OrderConfirmation>> PlaceOrder(CreateOrderRequest request)
{
    return await ValidateRequest(request)
        .Ensure(r => r.TotalAmount > 0, Error.Validation("Total must be positive"))
        .EnsureAsync(async r => await HasSufficientCredit(r.CustomerId, r.TotalAmount),
            Error.Conflict("Insufficient credit"))
        .BindAsync(async r => await CreateOrderAsync(r))
        .MapAsync(async order => await GenerateConfirmationAsync(order));
}
```

**Ensure Behavior:**
- **Short-circuit**: If result is already a failure, validation is skipped
- **Fail-fast**: Stops at first failed predicate
- **Preserves errors**: Original errors propagate unchanged
- **Chainable**: Multiple validations can be chained fluently

**When to use Ensure:**
- Business rule validation in pipelines
- Conditional checks that can fail with specific errors
- Guard clauses in functional style
- Alternative to early-return if-checks

### Async Operations (MapAsync / BindAsync / MatchAsync)

Handle asynchronous operations in Result pipelines:

```csharp
// MapAsync - transform value asynchronously
var result = Result.Success(42);
var mapped = await result.MapAsync(async x =>
{
    await Task.Delay(100);
    return x.ToString();
});

// BindAsync - chain async operations
var userResult = await GetUserIdAsync()
    .BindAsync(async id => await LoadUserAsync(id))
    .MapAsync(async user => await user.ToDisplayNameAsync());

// Working with Task<Result<T>>
Task<Result<User>> userTask = GetUserAsync(userId);

var emailResult = await userTask
    .MapAsync(user => user.Email)           // Sync transformation
    .BindAsync(async email => await ValidateEmailAsync(email)); // Async validation

// Real-world async pipeline
public async Task<Result<string>> ProcessUserRegistrationAsync(RegisterRequest request)
{
    return await ValidateRequestAsync(request)
        .BindAsync(async req => await CheckEmailAvailabilityAsync(req.Email))
        .BindAsync(async req => await CreateUserAsync(req))
        .MapAsync(async user => await SendWelcomeEmailAsync(user))
        .MapAsync(async success => "User registered successfully");
}

// Error handling in async operations
var result = await GetDataAsync()
    .MapAsync(async data =>
    {
        try
        {
            return await ProcessDataAsync(data);
        }
        catch (Exception ex)
        {
            return Result.Unexpected("PROCESSING_ERROR", ex.Message);
        }
    });
```

**Async Method Types:**
- `MapAsync<T, TOut>(Result<T>, Func<T, Task<TOut>>)` - Transform value asynchronously
- `MapAsync<T, TOut>(Task<Result<T>>, Func<T, TOut>)` - Transform from async result
- `BindAsync<T, TOut>(Result<T>, Func<T, Task<Result<TOut>>>)` - Chain async operations
- `BindAsync<T, TOut>(Task<Result<T>>, Func<T, Result<TOut>>)` - Chain from async result
- `EnsureAsync<T>(Result<T>, Func<T, Task<bool>>, Error)` - Async validation
- `EnsureAsync<T>(Task<Result<T>>, Func<T, bool>, Error)` - Validate async result
- `MatchAsync<T, TOut>(Result<T>, Func<T, Task<TOut>>, Func<Error, Task<TOut>>)` - Async pattern matching
- `MatchAsync<T, TOut>(Task<Result<T>>, Func<T, TOut>, Func<Error, TOut>)` - Match async result
- `MatchAsync<T, TOut>(Task<Result<T>>, Func<T, Task<TOut>>, Func<Error, Task<TOut>>)` - Fully async match

All async methods use `ConfigureAwait(false)` for optimal performance in ASP.NET scenarios.

### MatchAsync - Asynchronous Pattern Matching

Handle success and failure cases asynchronously:

```csharp
// Basic async matching
var result = Result.Success(42);
var message = await result.MatchAsync(
    onSuccess: async value =>
    {
        await LogSuccessAsync(value);
        return $"Success: {value}";
    },
    onFailure: async error =>
    {
        await LogErrorAsync(error);
        return $"Error: {error.Description}";
    }
);

// Async API call with MatchAsync
public async Task<IActionResult> GetUserAsync(int userId)
{
    return await GetUserByIdAsync(userId)
        .MatchAsync(
            onSuccess: async user =>
            {
                await AuditAsync($"User {userId} accessed");
                return Ok(user);
            },
            onFailure: async error =>
            {
                await LogErrorAsync(error);
                return error.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(error),
                    ErrorType.Unauthorized => Unauthorized(),
                    _ => StatusCode(500, error)
                };
            }
        );
}

// Chain async operations with MatchAsync
var finalResult = await LoadDataAsync(id)
    .MapAsync(async data => await ProcessAsync(data))
    .MatchAsync(
        onSuccess: async processed =>
        {
            await SaveAsync(processed);
            return "Processing completed successfully";
        },
        onFailure: async error =>
        {
            await NotifyAdminAsync(error);
            return $"Processing failed: {error.Description}";
        }
    );

// Working with Task<Result<T>>
Task<Result<User>> userTask = GetUserAsync(userId);

var response = await userTask.MatchAsync(
    onSuccess: user => new { Success = true, Data = user },
    onFailure: error => new { Success = false, Error = error.Description }
);

// Complex async transformations
var result = await ValidateInputAsync(input)
    .BindAsync(async validated => await ProcessAsync(validated))
    .BindAsync(async processed => await StoreAsync(processed))
    .MatchAsync(
        onSuccess: async stored =>
        {
            await Task.WhenAll(
                NotifySuccessAsync(stored),
                UpdateCacheAsync(stored),
                SendEmailAsync(stored)
            );
            return new SuccessResponse(stored.Id);
        },
        onFailure: async error =>
        {
            await CompensateAsync(error);
            return new ErrorResponse(error);
        }
    );
```

**MatchAsync Variants:**
- **`Result.MatchAsync`** - Match with both async handlers
- **`Task<Result>.MatchAsync`** - Match async result with sync handlers
- **`Task<Result>.MatchAsync`** - Match async result with async handlers
- **`Result<T>.MatchAsync`** - Generic result with async handlers
- **`Task<Result<T>>.MatchAsync`** - All combinations supported

**When to use MatchAsync:**
- API endpoints that need async logging/auditing
- Complex error handling with async notification
- Transforming results to HTTP responses asynchronously
- Orchestrating multiple async side effects

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
        .Ensure(order => order.Items.Any(), Error.Validation("Order must contain items"))
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

### Validation Pattern with Ensure

Alternative to early returns using `Ensure` for validation:

```csharp
// Before (manual checking)
public Result<User> GetActiveUser(int id)
{
    var userResult = GetUser(id);
    if (userResult.IsFailure)
        return userResult;

    var user = userResult.Value;
    if (!user.IsActive)
        return Error.Validation("User is not active");

    if (user.IsDeleted)
        return Error.NotFound("User was deleted");

    return Result.Success(user);
}

// After (with Ensure)
public Result<User> GetActiveUser(int id)
{
    return GetUser(id)
        .Ensure(user => user.IsActive, Error.Validation("User is not active"))
        .Ensure(user => !user.IsDeleted, Error.NotFound("User was deleted"));
}

// Complex validation pipeline
public async Task<Result<Transaction>> ProcessPayment(PaymentRequest request)
{
    return await ValidatePaymentRequest(request)
        .Ensure(req => req.Amount > 0, Error.Validation("Amount must be positive"))
        .Ensure(req => req.Amount <= 10000, Error.Validation("Amount exceeds limit"))
        .EnsureAsync(
            async req => await HasSufficientFunds(req.AccountId, req.Amount),
            Error.Conflict("Insufficient funds"))
        .EnsureAsync(
            async req => await IsAccountActive(req.AccountId),
            Error.Forbidden("Account is not active"))
        .BindAsync(async req => await CreateTransactionAsync(req));
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

Kubis1982
https://github.com/kubis1982/Result
