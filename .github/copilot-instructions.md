# GitHub Copilot Instructions for Result Project

## Project Overview
**Repository**: kubis1982/Result  
**Technology**: .NET 10, C# 14.0  
**Pattern**: Result/Railway Oriented Programming  
**Purpose**: Functional error handling library with ASP.NET and WolverineFx integrations

## Code Generation Standards

### Language & Framework
- Target: **.NET 10**
- C# Language Version: **14.0**
- Use modern C# features: file-scoped namespaces, record types, primary constructors
- Nullable reference types enabled

### Code Style
- Use **implicit usings** where appropriate
- Prefer **expression-bodied members** for simple methods
- Use **pattern matching** over traditional if-else when clearer
- Follow **Railway Oriented Programming** principles
- Immutability by default (readonly, init-only properties)

### Documentation
- XML documentation for **all public APIs**
- Include `<summary>`, `<param>`, `<returns>` tags
- Use `<see cref=""/>` for cross-references
- Examples in `<example>` tags for complex methods

### Naming Conventions
- Public methods: PascalCase
- Private methods: camelCase
- Constants: PascalCase
- Error codes: PascalCase (e.g., `ErrorCodes.NotFound`)
- Avoid abbreviations unless widely known

## Project Structure

```
src/
├── Result/              → Core library (scope: result)
├── Result.AspNet/       → ASP.NET integration (scope: aspnet)
└── Result.WolverineFx/  → WolverineFx integration (scope: wolverine)
tests/
├── Result.Tests/
├── Result.AspNet.Tests/
└── Result.WolverineFx.Tests/
```

## Commit Message Rules

### Format
```
<type>(<scope>): <subject>
```

### Types
- `feat` - New feature (minor version bump)
- `fix` - Bug fix (patch version bump)
- `docs` - Documentation only
- `test` - Adding/updating tests
- `refactor` - Code restructuring, no behavior change
- `perf` - Performance improvement
- `build` - Dependencies, build system
- `ci` - CI/CD changes
- `chore` - Maintenance tasks

### Scopes
- `result` - Core Result library (`src/Result/`)
- `aspnet` - ASP.NET integration (`src/Result.AspNet/`)
- `wolverine` - WolverineFx integration (`src/Result.WolverineFx/`)
- `tests` - Test projects
- `deps` - Dependencies (*.csproj)
- `ci` - GitHub workflows (`.github/workflows/`)
- `docs` - Documentation (*.md)

### Subject Rules
1. **Imperative mood**: "add" not "added"
2. **Lowercase** after colon
3. **No period** at end
4. **Max 72 characters**
5. **Be specific**: mention method/class names
6. **Never**: WIP, update, changes, fix bug

### Breaking Changes
Add `!` after scope:
```
feat(result)!: change Match method signature
```

### Examples
```
feat(result): add TryMap method for exception-safe mapping
fix(aspnet): prevent null reference in error middleware
docs(readme): add WolverineFx integration guide
test(result): add comprehensive Match method tests
refactor(wolverine): extract handler factory class
perf(result): reduce allocations in Map method
build(deps): upgrade WolverineFx to 2.1.0
ci(release): automate changelog generation
chore(release): bump version to 1.2.3
```

## Code Generation Guidelines

### Result Type Patterns

When generating code for the Result type:

```csharp
// ✅ Good: Railway-oriented style
public Result<User> GetUser(int id)
{
    var user = _repository.Find(id);
    return user is not null 
        ? Result.Success(user)
        : Result.Failure<User>(Error.NotFound("User not found"));
}

// ✅ Good: Chaining operations
return GetUser(id)
    .Map(user => user.ToDto())
    .Match(
        onSuccess: dto => Ok(dto),
        onFailure: error => NotFound(error.Description)
    );
```

### Error Creation Patterns

```csharp
// ✅ Good: Use factory methods
Error.NotFound("description")
Error.Conflict("description")
Error.Validation("description")

// ✅ Good: Custom codes
Error.NotFound("User.Missing", "User with ID 123 not found")
Error.Validation("Email.Invalid", "Email address is malformed")
```

### Extension Method Patterns

```csharp
// ✅ Good: Follow existing patterns
public static Result<TOut> Map<TIn, TOut>(
    this Result<TIn> result, 
    Func<TIn, TOut> mapper)
{
    return result.IsSuccess
        ? Result.Success(mapper(result.Value))
        : Result.Failure<TOut>(result.Error);
}
```

### ASP.NET Integration Patterns

```csharp
// ✅ Good: Extension methods for IResult
public static IResult ToHttpResult(this Result result)
{
    return result.Match(
        onSuccess: () => Results.Ok(),
        onFailure: error => error.ErrorType switch
        {
            ErrorType.NotFound => Results.NotFound(error),
            ErrorType.Validation => Results.BadRequest(error),
            ErrorType.Conflict => Results.Conflict(error),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.Forbidden => Results.Forbid(),
            _ => Results.Problem(error.Description)
        }
    );
}
```

### WolverineFx Integration Patterns

```csharp
// ✅ Good: Handler returning Result
public class CreateOrderHandler
{
    public Result<OrderCreated> Handle(CreateOrder command)
    {
        // Validation
        if (command.Amount <= 0)
            return Error.Validation("Amount must be positive");
            
        // Business logic
        var order = Order.Create(command);
        
        return new OrderCreated(order.Id);
    }
}
```

## Testing Guidelines

### Test Naming Philosophy: Sentence Style

This project uses **Sentence Style** for test naming that reads like natural English sentences.

**Our Approach:**
- ✅ **Use**: `Should_ExpectedBehavior_When_Context` (Natural sentence)
- Format: Test names should read as complete sentences
- Example: `Should_CallOnSuccessFunction_When_ResultIsSuccess`
- Example: `Should_ThrowException_When_AmountIsTooHigh`

**Why Sentence Style?**
1. **Readable**: Reads like plain English, easy to understand
2. **Natural**: Follows how we naturally describe behavior
3. **Clear**: Immediately tells you what the test verifies
4. **Self-documenting**: Test name explains the scenario completely
5. **Business-friendly**: Non-technical stakeholders can understand test names

**Pattern Structure:**

| Component | Example | Purpose |
|-----------|---------|----------|
| Should | `Should` | Indicates expected behavior |
| Expected Behavior | `CallOnSuccessFunction` | What should happen |
| When | `When` | Separates behavior from context |
| Context | `ResultIsSuccess` | The scenario/state |

**Full Examples:**
- `Should_CallOnSuccessFunction_When_ResultIsSuccess`
- `Should_CallOnFailureFunction_When_ResultIsFailure`
- `Should_TransformToAnyType_When_UsingMatch`
- `Should_PreserveError_When_MappingFailedResult`
- `Should_ThrowException_When_AccessingValueOfFailedResult`

**Key Principles:**
- Names are complete **sentences** describing the test
- Use **Should** to start the expected behavior
- Use **When** to introduce the context/condition
- Write in **present tense** (not past: "Called", but "Calls")
- Be **specific** about what is being tested
- Maintain **consistent structure** across all tests

### Test Structure

**Always use Arrange-Act-Assert pattern with explicit comments:**

```csharp
[Fact]
public void Should_CallOnSuccessFunction_When_ResultIsSuccess()
{
    // Arrange
    var result = Result.Success(42);

    // Act
    var output = result.Match(
        onSuccess: value => $"Value: {value}",
        onFailure: error => $"Error: {error.Description}"
    );

    // Assert
    Assert.Equal("Value: 42", output);
}

[Fact]
public void Should_CallOnFailureFunction_When_ResultIsFailure()
{
    // Arrange
    var error = Error.NotFound("User not found");
    var result = Result.Failure<int>(error);

    // Act
    var output = result.Match(
        onSuccess: value => $"Value: {value}",
        onFailure: err => $"Error: {err.Description}"
    );

    // Assert
    Assert.Equal("Error: User not found", output);
}

[Fact]
public void Should_TransformToAnyType_When_UsingMatchWithSuccessResult()
{
    // Arrange
    var result = Result.Success(42);

    // Act
    var transformed = result.Match(
        onSuccess: value => new { Number = value, IsEven = value % 2 == 0 },
        onFailure: error => new { Number = 0, IsEven = false }
    );

    // Assert
    Assert.Equal(42, transformed.Number);
    Assert.True(transformed.IsEven);
}
```

### Async Test Structure

For async tests, follow the same naming pattern:

```csharp
[Fact]
public async Task Should_CallOnSuccessFunction_When_ResultIsSuccessAsync()
{
    // Arrange
    var result = Result.Success(42);

    // Act
    var output = await result.MatchAsync(
        onSuccess: async value =>
        {
            await Task.Delay(1);
            return $"Value: {value}";
        },
        onFailure: async error =>
        {
            await Task.Delay(1);
            return $"Error: {error.Description}";
        }
    );

    // Assert
    Assert.Equal("Value: 42", output);
}

[Fact]
public async Task Should_CallOnSuccessFunction_When_UsingTaskResultWithAsyncFunctions()
{
    // Arrange
    var resultTask = Task.FromResult(Result.Success());

    // Act
    var output = await resultTask.MatchAsync(
        onSuccess: async () =>
        {
            await Task.Delay(1);
            return "Success";
        },
        onFailure: async error =>
        {
            await Task.Delay(1);
            return $"Error: {error.Description}";
        }
    );

    // Assert
    Assert.Equal("Success", output);
}
```

### Assertion Style

**Use xUnit Assert class - NOT FluentAssertions:**

```csharp
// ✅ Good: xUnit Assert
Assert.True(result.IsSuccess);
Assert.False(result.IsFailure);
Assert.Equal(42, result.Value);
Assert.Equal("Expected", actual);
Assert.Throws<ResultException>(() => failedResult.Value);

// ❌ Bad: FluentAssertions (not used in this project)
result.IsSuccess.Should().BeTrue();
result.Value.Should().Be(42);
```

### Theory Tests for Multiple Scenarios
```csharp
[Theory]
[InlineData(ErrorType.NotFound, 404)]
[InlineData(ErrorType.Validation, 400)]
[InlineData(ErrorType.Conflict, 409)]
public void Should_MapToCorrectStatusCode_When_ErrorTypeIsProvided(ErrorType type, int expected)
{
    // Arrange
    var error = new Error { ErrorType = type };

    // Act
    var statusCode = error.ToStatusCode();

    // Assert
    Assert.Equal(expected, statusCode);
}
```

## Common Patterns to Follow

### 1. Immutability
```csharp
// ✅ Use init-only properties
public record Error
{
    public ErrorType ErrorType { get; init; }
    public string Code { get; init; }
    public string Description { get; init; }
}
```

### 2. Factory Methods
```csharp
// ✅ Private constructor + public factory methods
public readonly record struct Error
{
    private Error(ErrorType errorType, string code, string description)
    {
        ErrorType = errorType;
        Code = code;
        Description = description;
    }
    
    public static Error NotFound(string description) => 
        new(ErrorType.NotFound, ErrorCodes.NotFound, description);
}
```

### 3. Extension Methods
```csharp
// ✅ Use extension methods for fluent API
public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result, 
        Func<TIn, TOut> mapper)
    {
        // Implementation
    }
}
```

### 4. Pattern Matching
```csharp
// ✅ Use switch expressions
return error.ErrorType switch
{
    ErrorType.NotFound => Results.NotFound(),
    ErrorType.Validation => Results.BadRequest(),
    ErrorType.Conflict => Results.Conflict(),
    _ => Results.Problem()
};
```

## Anti-Patterns to Avoid

❌ Throwing exceptions for business logic errors  
❌ Using null instead of Result.Failure  
❌ Ignoring error information  
❌ Mixing Result with nullable patterns  
❌ Creating mutable Result types  
❌ Using generic Exception catching  

## Code Review Checklist

When generating or reviewing code:

- [ ] Follows Railway Oriented Programming principles
- [ ] All public APIs have XML documentation
- [ ] Nullable reference types handled correctly
- [ ] No unnecessary allocations
- [ ] Immutability maintained
- [ ] Error messages are descriptive
- [ ] Tests cover success and failure paths
- [ ] Breaking changes are documented
- [ ] Consistent with existing codebase style

## Additional Resources

For more details, see:
- `.copilot-instructions.md` - Quick commit message reference
- `.github/claude.md` - Detailed AI instructions
- Project README files in each package

## Remember

This is a **functional programming library** focused on:
1. **Type safety** over runtime exceptions
2. **Railway Oriented Programming** for clear success/failure paths
3. **Composability** through monadic operations
4. **Immutability** for predictable behavior
5. **Integration** with ASP.NET and WolverineFx ecosystems

Generate code that developers will be happy to use and maintain!
