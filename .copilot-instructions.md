# GitHub Copilot - Commit Message Rules for Git Changes

## Format: <type>(<scope>): <subject>

## Types: feat fix docs test refactor perf build ci chore
## Scopes: result aspnet wolverine tests deps ci docs

## Scope by File Path:
- src/Result/ -> result
- src/Result.AspNet/ -> aspnet
- src/Result.WolverineFx/ -> wolverine
- tests/ -> tests
- *.csproj -> deps
- .github/workflows/ -> ci
- *.md -> docs

## Examples:
- feat(result): add TryMap method for exception-safe mapping
- fix(aspnet): prevent null reference in error middleware
- docs(readme): add WolverineFx integration guide
- test(result): add comprehensive Match method tests
- refactor(wolverine): extract handler factory class
- perf(result): reduce allocations in Map method
- build(deps): upgrade WolverineFx to 2.1.0
- ci(release): automate changelog generation
- chore(release): bump version to 1.2.3

## Rules:
- Use imperative: add NOT added
- Lowercase after colon
- No period at end
- Max 72 characters
- Specific: name the method/class
- NEVER: WIP, update, changes, fix bug

## Breaking Changes: Add ! after scope
Example: feat(result)!: change Match method signature

See .github/copilot.md for complete guide
