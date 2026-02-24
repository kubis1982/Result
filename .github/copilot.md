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

### Testing
- Use xUnit framework
- Test method names: should follow pattern `MethodName_Scenario_ExpectedBehavior`
- Arrange-Act-Assert pattern
- FluentAssertions for readable assertions

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

### Test Structure
```csharp
[Fact]
public void Map_WithSuccessResult_TransformsValue()
{
    // Arrange
    var result = Result.Success(42);
    
    // Act
    var mapped = result.Map(x => x.ToString());
    
    // Assert
    mapped.IsSuccess.Should().BeTrue();
    mapped.Value.Should().Be("42");
}

[Fact]
public void Map_WithFailureResult_PreservesError()
{
    // Arrange
    var error = Error.NotFound("Not found");
    var result = Result.Failure<int>(error);
    
    // Act
    var mapped = result.Map(x => x.ToString());
    
    // Assert
    mapped.IsFailure.Should().BeTrue();
    mapped.Error.Should().Be(error);
}
```

### Theory Tests for Multiple Scenarios
```csharp
[Theory]
[InlineData(ErrorType.NotFound, 404)]
[InlineData(ErrorType.Validation, 400)]
[InlineData(ErrorType.Conflict, 409)]
public void ToStatusCode_MapsErrorTypeCorrectly(ErrorType type, int expected)
{
    var error = new Error { ErrorType = type };
    error.ToStatusCode().Should().Be(expected);
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
