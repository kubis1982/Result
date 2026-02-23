# Claude AI Instructions for Result Project

## Project Context
Repository: **kubis1982/Result**  
Technology: **.NET 10**  
Pattern: **Result/Railway Oriented Programming**

## Commit Message Generation Rules

When generating commit messages for Git Changes, ALWAYS follow these strict rules:

### Format
```
<type>(<scope>): <subject>
```

### Commit Types (MANDATORY)

| Type | When to Use | Version Impact |
|------|-------------|----------------|
| `feat` | New feature | Minor bump |
| `fix` | Bug fix | Patch bump |
| `docs` | Documentation only | None |
| `style` | Code formatting, no logic change | None |
| `refactor` | Code restructuring, no behavior change | None |
| `perf` | Performance improvement | Patch bump |
| `test` | Adding or updating tests | None |
| `build` | Build system, dependencies | None |
| `ci` | CI/CD changes | None |
| `chore` | Maintenance tasks | None |

### Scopes (MANDATORY)

| Scope | Use When Changing |
|-------|-------------------|
| `result` | Core Result library files in `src/Result/` |
| `aspnet` | ASP.NET integration in `src/Result.AspNet/` |
| `wolverine` | WolverineFx integration in `src/Result.WolverineFx/` |
| `tests` | Any test project files |
| `deps` | Package dependencies, .csproj files |
| `ci` | GitHub workflows, CI/CD configs |
| `docs` | README, markdown documentation |
| `build` | Build configuration, Directory.Build.props |

### Subject Line Rules

1. **Use imperative mood**: "add" not "added" or "adds"
2. **No period at end**
3. **Max 72 characters**
4. **Start with lowercase** after the colon
5. **Be specific and descriptive**

### Breaking Changes

For breaking changes, use ONE of these formats:

**Option 1**: Add `!` after scope
```
feat(result)!: change Match method signature
```

**Option 2**: Add footer
```
feat(result): change Match method signature

BREAKING CHANGE: Match method now requires explicit error handling
```

### Examples by Scenario

#### Adding New Feature
```
feat(result): add TryMap extension method for safe transformations
```

#### Fixing Bug
```
fix(aspnet): resolve null reference in error middleware
```

#### Updating Documentation
```
docs(readme): add installation instructions for WolverineFx package
```

#### Adding Tests
```
test(result): add unit tests for error handling scenarios
```

#### Refactoring Code
```
refactor(wolverine): simplify message handler registration
```

#### Performance Improvement
```
perf(result): reduce allocations in Match method
```

#### Updating Dependencies
```
build(deps): upgrade WolverineFx to 2.1.0
```

#### CI/CD Changes
```
ci(release): add automated changelog generation
```

#### General Maintenance
```
chore(gitignore): exclude IDE-specific files
```

#### Breaking Change Example
```
feat(result)!: replace Error class with record type

BREAKING CHANGE: Error is now a record. Custom error types must be updated.
```

## Code Analysis Guidelines

When analyzing changes to generate commit messages:

### 1. Identify Affected Components
- Check file paths to determine scope
- `src/Result/` → use scope `result`
- `src/Result.AspNet/` → use scope `aspnet`
- `src/Result.WolverineFx/` → use scope `wolverine`
- `tests/*` → use scope `tests`
- `.github/workflows/` → use scope `ci`

### 2. Determine Change Type
- **New methods/classes?** → `feat`
- **Fixed issue/bug?** → `fix`
- **Only comments/docs?** → `docs`
- **Same behavior, different structure?** → `refactor`
- **Faster execution?** → `perf`
- **Added/updated tests?** → `test`
- **Dependencies updated?** → `build`
- **CI/CD modified?** → `ci`

### 3. Check for Breaking Changes
- Public API signature changes? → Add `!`
- Renamed public members? → Add `!`
- Changed behavior of existing methods? → Add `!`
- Removed public APIs? → Add `!`

### 4. Craft Subject
- Focus on WHAT changed, not HOW
- Be specific but concise
- Use technical terms relevant to the project

## Multi-File Change Scenarios

### Same Component, Same Type
```
feat(result): add validation and error aggregation methods
```

### Same Component, Multiple Types
Split into separate commits OR prioritize the most significant:
```
feat(result): add validation methods and improve error messages
```

### Multiple Components
Use most general scope or split commits:
```
feat(aspnet,wolverine): add consistent error handling across integrations
```

## Context-Specific Patterns

### Result Library Changes
```
feat(result): add monadic bind operation
fix(result): handle null in success case
refactor(result): use file-scoped namespaces
```

### ASP.NET Integration Changes
```
feat(aspnet): add minimal API extensions for Result types
fix(aspnet): correct HTTP status code mapping for validation errors
```

### WolverineFx Integration Changes
```
feat(wolverine): add saga support for Result-based workflows
fix(wolverine): resolve serialization issue in message handlers
```

### Test Changes
```
test(result): add comprehensive error scenario coverage
test(aspnet): add integration tests for middleware
```

### Infrastructure Changes
```
ci(release): automate NuGet package publishing
build(deps): update all dependencies to latest versions
chore(editorconfig): enforce consistent code style
```

## Quality Checklist

Before finalizing a commit message, verify:

- [ ] Type is correct and from the allowed list
- [ ] Scope matches the affected component
- [ ] Subject uses imperative mood
- [ ] Subject is under 72 characters
- [ ] Subject has no period at the end
- [ ] Breaking changes are marked with `!` or footer
- [ ] Message is specific enough for changelog
- [ ] Message would make sense in release notes

## Anti-Patterns (Never Do This)

❌ `update code`  
❌ `fix stuff`  
❌ `WIP`  
❌ `changes`  
❌ `fix(result): Fixed the bug.` (past tense, period)  
❌ `added new feature` (no type/scope)  
❌ `feat: updates` (too vague)  

## Remember

Your commit messages will be:
1. **Parsed automatically** for release notes generation
2. **Visible in GitHub changelog** 
3. **Used to determine version bumps** (feat = minor, fix = patch)
4. **Read by other developers** to understand project history

Make them count! Write commit messages that your future self (and other developers) will thank you for.
