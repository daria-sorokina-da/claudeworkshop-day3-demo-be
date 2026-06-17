# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

@import .claude/code-style.md

## Architecture

Layered design: HTTP requests enter via **Controllers**, which delegate all logic to **Services**. Services operate on plain **Models**. There is no database — the in-memory store lives inside `HorseService`, registered as a singleton.

```
Request → HorsesController → IHorseService → HorseService (Dictionary<int, Horse>)
```

> **Note:** `code-style.md` specifies FluentValidation for request validation, but it is not yet wired up — controllers currently do manual validation inline.

## Domain Model

Single aggregate: **`Horse`** (`src/StableApi/Models/Horse.cs`)

| Field | Type | Notes |
|-------|------|-------|
| `Id` | `int` | Auto-incremented by `HorseService` |
| `Name` | `string` | |
| `OwnerEmail` | `string` | |
| `Breed` | `string` | |
| `RegisteredAt` | `DateTime` | Set to `DateTime.UtcNow` on create |
| `IsActive` | `bool` | Soft-delete flag — `Delete` sets this to `false` rather than removing the record |

Request DTOs (`src/StableApi/Models/HorseRequests.cs`): `CreateHorseRequest` and `UpdateHorseRequest` are C# records with `Name`, `OwnerEmail`, and `Breed`.

**Seed data:** 5 horses are pre-loaded on startup — Moonbeam, Thunderhoof, Clover, Pippin, Solstice.

## Running the Project

```bash
# Restore & build
dotnet build

# Run the API  (Swagger UI → http://localhost:5000/swagger)
dotnet run --project src/StableApi

# Run all tests
dotnet test

# Run a single test by name
dotnet test --filter "FullyQualifiedName~GetById_NonExistentId"

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

| File | Purpose |
|------|---------|
| `src/StableApi/Controllers/HorsesController.cs` | HTTP layer — routes, status codes, delegates to service |
| `src/StableApi/Services/IHorseService.cs` | Service contract |
| `src/StableApi/Services/HorseService.cs` | In-memory store + all business logic |
| `src/StableApi/Models/Horse.cs` | Domain entity |
| `src/StableApi/Models/HorseRequests.cs` | Request DTOs |
| `src/StableApi/Program.cs` | DI wiring and middleware setup |
| `tests/StableApi.Tests/HorsesControllerTests.cs` | Unit tests (xUnit + Moq) |
| `.claude/code-style.md` | Coding conventions — naming, HTTP status codes, validation rules |

## Test Patterns

- **Framework:** xUnit + Moq
- **Location:** `tests/StableApi.Tests/`
- `IHorseService` is mocked; the controller under test receives the mock via constructor injection
- **Naming:** `MethodName_Scenario_ExpectedBehaviour` — e.g. `GetById_NonExistentId_ReturnsNotFound`
- **Structure:** Arrange (configure mock) → Act (call controller method) → Assert (check `IActionResult` type and value)
