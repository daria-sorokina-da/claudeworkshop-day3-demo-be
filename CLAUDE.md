# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Enchanted Stables Registry API

@import .claude/code-style.md

## Architecture

Three-layer ASP.NET Core 8 Web API with strict separation of concerns:

- **Controllers** — thin HTTP adapters only; delegate all logic to services; return appropriate status codes
- **Services** — all business logic lives here; the interface (`IHorseService`) is the boundary
- **Models** — plain data containers: domain entities (`Horse`) and request DTOs (`CreateHorseRequest`, `UpdateHorseRequest`)

The service layer uses an in-memory `Dictionary<int, Horse>` with auto-increment IDs and soft-delete (`IsActive = false`). No database — persistence resets on restart.

## Domain Model

**Horse** — the only aggregate root:
- `Id` (int, auto-assigned), `Name`, `OwnerEmail`, `Breed` (string)
- `RegisteredAt` (DateTime, set on creation), `IsActive` (bool, soft-delete flag)

**Request DTOs** (C# records, no behavior):
- `CreateHorseRequest(Name, OwnerEmail, Breed)`
- `UpdateHorseRequest(Name, OwnerEmail, Breed)`

## Running the Project

```bash
# Restore & build
dotnet build

# Run the API  (Swagger UI → http://localhost:5000/swagger)
dotnet run --project src/StableApi

# Run all tests
dotnet test

# Run a single test by name filter
dotnet test --filter "FullyQualifiedName~MethodName_Scenario"

# Format source code
dotnet format src/StableApi/StableApi.csproj
```

## Solution Structure

```
src/StableApi/         — ASP.NET Web API
  Controllers/         — HTTP layer only
  Services/            — Business logic + in-memory store
  Models/              — Domain entities and request DTOs
tests/StableApi.Tests/ — xunit unit tests (Moq)
infrastructure/        — Terraform for Azure deployment
```

## Key Files

- `src/StableApi/Services/IHorseService.cs` — the service contract; extend this when adding new operations
- `src/StableApi/Services/HorseService.cs` — all domain logic and in-memory store
- `src/StableApi/Controllers/HorsesController.cs` — HTTP layer; should stay thin
- `src/StableApi/Models/Horse.cs` — domain entity
- `src/StableApi/Models/HorseRequests.cs` — request DTOs
- `.claude/code-style.md` — naming conventions, HTTP status code rules, validation patterns

## Test Patterns

xunit + Moq; tests live in `tests/StableApi.Tests/HorsesControllerTests.cs`.

**Naming:** `MethodName_Scenario_ExpectedBehaviour`

**Structure:** constructor creates `Mock<IHorseService>` and the controller under test; each test follows Arrange / Act / Assert with one logical assertion.

```csharp
[Fact]
public void GetById_ExistingHorse_ReturnsOk()
{
    // Arrange
    _mockService.Setup(s => s.GetById(1)).Returns(new Horse { Id = 1, Name = "Shadowfax" });

    // Act
    var result = _controller.GetById(1);

    // Assert
    Assert.IsType<OkObjectResult>(result);
}
```

Always mock `IHorseService` — never instantiate `HorseService` directly in tests.
