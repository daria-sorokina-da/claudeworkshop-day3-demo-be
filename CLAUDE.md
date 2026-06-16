# Enchanted Stables Registry API

@import .claude/code-style.md

## Architecture

<!-- TODO (workshop exercise): describe the solution layers and key design decisions -->

## Domain Model

<!-- TODO (workshop exercise): list the main entities and their relationships -->

## Running the Project

```bash
# Restore & build
dotnet build

# Run the API  (Swagger UI → http://localhost:5000/swagger)
dotnet run --project src/StableApi

# Run all tests
dotnet test

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

<!-- TODO (workshop exercise): call out the most important files and why -->

## Test Patterns

<!-- TODO (workshop exercise): describe how tests are structured in this project -->
