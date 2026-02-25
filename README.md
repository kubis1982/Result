# Result

<div align="center">

[![NuGet - Result](https://img.shields.io/nuget/v/Kubis1982.Result.svg?label=Kubis1982.Result)](https://www.nuget.org/packages/Kubis1982.Result/)
[![NuGet - AspNet](https://img.shields.io/nuget/v/Kubis1982.Result.AspNet.svg?label=Kubis1982.Result.AspNet)](https://www.nuget.org/packages/Kubis1982.Result.AspNet/)
[![NuGet - WolverineFx](https://img.shields.io/nuget/v/Kubis1982.Result.WolverineFx.svg?label=Kubis1982.Result.WolverineFx)](https://www.nuget.org/packages/Kubis1982.Result.WolverineFx/)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Release](https://github.com/kubis1982/Result/actions/workflows/release.yml/badge.svg)](https://github.com/kubis1982/Result/actions/workflows/release.yml)

</div>

A lightweight implementation of the Result pattern for .NET, providing type-safe error handling without exceptions.

---

## 📦 Packages

This repository contains three NuGet packages:

| Package | Description | Documentation |
|---------|-------------|---------------|
| **Kubis1982.Result** | Core Result pattern library | [📖 Documentation](src/Result/README.md) |
| **Kubis1982.Result.AspNet** | ASP.NET Core integration | [📖 Documentation](src/Result.AspNet/README.md) |
| **Kubis1982.Result.WolverineFx** | WolverineFx integration | [📖 Documentation](src/Result.WolverineFx/README.md) |

---

## 🚀 Quick Start

### Installation

```bash
# Core library
dotnet add package Kubis1982.Result

# ASP.NET Core integration
dotnet add package Kubis1982.Result.AspNet

# WolverineFx integration
dotnet add package Kubis1982.Result.WolverineFx
```

## ✨ Features

- **Type-safe error handling**: Use `Result` and `Result<T>` types to represent success or failure without throwing exceptions
- **Railway-oriented programming**: Build robust error-handling pipelines
- **Minimal dependencies**: Pure .NET implementation with no external packages
- **ASP.NET Core integration**: Seamless conversion to IResult and ProblemDetails
- **WolverineFx integration**: Message handler support with automatic continuation strategies
- **Rich error types**: Structured `Error` with codes, descriptions, and error types
- **Tuple deconstruction**: Use pattern matching with `Deconstruct` method
- **TryGetValue pattern**: Safe value extraction without exceptions
- **Combine operations**: Compose multiple results with fail-fast behavior

---

## 📚 Documentation

Each package has its own detailed documentation:

- **[Kubis1982.Result](src/Result/README.md)** - Core library with Result pattern implementation
- **[Kubis1982.Result.AspNet](src/Result.AspNet/README.md)** - ASP.NET Core extensions and HTTP integration
- **[Kubis1982.Result.WolverineFx](src/Result.WolverineFx/README.md)** - WolverineFx message handling integration

---

## 🧪 Testing

The project includes comprehensive test suites for all packages:

### Test Projects

- **Kubis1982.Result.Tests** - Unit tests for the core Result library
- **Kubis1982.Result.AspNet.Tests** - Unit tests for ASP.NET Core extensions
- **Kubis1982.Result.WolverineFx.Tests** - Integration tests for WolverineFx extensions

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test tests/Result.Tests/Kubis1982.Result.Tests.csproj
dotnet test tests/Result.AspNet.Tests/Kubis1982.Result.AspNet.Tests.csproj
dotnet test tests/Result.WolverineFx.Tests/Kubis1982.Result.WolverineFx.Tests.csproj
```

### Test Framework

- **xUnit v3** - Modern testing framework
- **Coverlet** - Code coverage collection
- **Microsoft.NET.Test.Sdk** - Test platform integration

---

## 🔄 Release Process

This repository uses automated CI/CD for publishing releases:

- **Continuous Integration**: All commits to `main` are automatically tested
- **Automated Releases**: Creating a version tag (e.g., `v1.2.0`) triggers the full release pipeline
- **NuGet Publishing**: Packages are automatically published to NuGet.org

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Mariusz Świątnicki**

- GitHub: [@kubis1982](https://github.com/kubis1982)
- Project: [Result](https://github.com/kubis1982/Result)