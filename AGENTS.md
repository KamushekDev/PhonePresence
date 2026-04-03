# Agent Guidelines for PresenceBot

## Project Overview

This is a .NET 10.0 C# application that monitors client presence on a network router and sends Telegram notifications. The solution follows clean architecture with separate layers for Core, Infrastructure, Services, and Integration.

## Build & Test Commands

### Notes
- All projects use **Central Package Management** via `Directory.Packages.props`
- All projects share common properties via `Directory.Build.props` (net10.0, nullable, implicit usings)
- Package versions are managed centrally - do not specify versions in individual `.csproj` files

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Run a Single Test
```bash
dotnet test --filter "FullyQualifiedName~TestClassName.MethodName"
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Build Release
```bash
dotnet build -c Release
```

### Database Migrations
```bash
# Add migration (from src directory)
dotnet ef migrations add MigrationName -s PresenceBot -p PresenceBot.Infrastructure -o Database/Migrations

# Apply migrations
dotnet ef database update -s PresenceBot -p PresenceBot.Infrastructure

# Reset migrations
dotnet ef database update 0 -s PresenceBot -p PresenceBot.Infrastructure

# Remove last migration
dotnet ef migrations remove -s PresenceBot -p PresenceBot.Infrastructure
```

## Project Structure

```
src/
├── PresenceBot/                          # Main web API project
├── PresenceBot.Core/                    # Domain models and interfaces (no dependencies)
├── PresenceBot.Infrastructure/           # EF Core, Telegram, background jobs
├── PresenceBot.Services/                # Business logic services
├── PresenceBot.Integration.Router/      # Router API integration
└── PresenceBot.Integration.Router.DependencyInjection/
tests/
└── PresenceBot.Tests/                   # xUnit unit tests
```

## Code Style Guidelines

### General
- All projects target .NET 10.0 with `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`
- Use **file-scoped namespaces** (`namespace X.Y;`)
- **No regions** - code should be organized logically without #region/#endregion
- **No comments** unless explaining complex business logic or non-obvious behavior

### Naming Conventions
- **Interfaces**: Prefix with `I` (e.g., `IClientPresenceService`)
- **Classes**: PascalCase
- **Private fields**: `_camelCase` (e.g., `_routerGateway`)
- **Methods**: PascalCase
- **Local variables**: camelCase
- **Directory structure**: Follow namespace hierarchy, use plural names for collections

### Types & Properties
- Prefer **records** for immutable DTOs and response models
- Use `required` keyword for required init properties:
  ```csharp
  public required string Identity { get; init; }
  ```
- Use `init` setters for properties that should only be set at construction
- Use ` CancellationToken` as last parameter in async methods

### Imports
- Use explicit imports (avoid global usings except standard .NET types via ImplicitUsings)
- Order imports: System namespaces first, then third-party, then project namespaces
- Group imports by type if using manual imports

### Dependency Injection
- Register services via **extension methods** on `IServiceCollection` in files named `ServiceCollectionExtensions.cs`
- Use `IOptions<T>` and `IOptionsSnapshot<T>` for configuration
- Use **named HttpClient** via `IHttpClientFactory` for HTTP clients

### Error Handling
- Create custom exception classes for domain-specific errors (e.g., `RouterLoginFailed`)
- Throw exceptions with descriptive messages
- Use `is not null` pattern for null checks when deserializing
- Propagate `CancellationToken` to all async operations

### Async/Await
- Use `async`/`await` for I/O operations (HTTP, database)
- Return `Task` or `Task<T>` from async methods
- Do not use `.Result` or `.Wait()` - always await

### Entity Framework Core
- Use Fluent API for entity configuration in `Configs/` folder
- Apply configurations via `ApplyConfigurationsFromAssembly`
- Use migrations for schema changes
- Use SQLite for local development

### Testing
- Use **xUnit** with `Fact` and `Theory` attributes
- Use descriptive test names: `MethodName_Scenario_ExpectedBehavior`
- Place tests in `tests/PresenceBot.Tests/` matching the project structure
- Follow Arrange-Act-Assert pattern

### Key Dependencies
- **Telegram.Bot** - Telegram API client
- **Microsoft.EntityFrameworkCore.Sqlite** - Database
- **Commandante** - Command/query handling
- **OneOf** - Type-safe discriminated unions
- **Newtonsoft.Json** - JSON serialization for Router integration
- **LinqSpecs** - Specification pattern for queries

### Configuration
- Store secrets in user secrets during development: `dotnet user-secrets set "Key:SubKey" "value"`
- Use `IConfiguration` and options pattern for settings
- Never commit secrets or credentials

## Common Patterns

### Service Registration Pattern
```csharp
public static class ServiceCollectionExtensions
{
    public static void AddMyFeature(this IServiceCollection services)
    {
        services.AddHttpClient<IMyClient, MyClient>("MyClient");
        services.AddScoped<IMyService, MyService>();
    }
}
```

### Record Pattern
```csharp
public record PresenceInfo(Client Client, DateTimeOffset Moment, bool Available);
```

### Required Property Pattern
```csharp
public class ClientPresence
{
    public required string Identity { get; init; }
    public required DateTimeOffset LastAvailableAt { get; init; }
}
```
