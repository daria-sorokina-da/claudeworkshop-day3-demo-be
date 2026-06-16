# C# Coding Style

## Naming
- Classes, methods, properties: PascalCase
- Parameters, local variables: camelCase
- Private fields: `_camelCase` (underscore prefix)
- Interfaces: `IPascalCase`
- Constants: PascalCase

## File Layout
- One type per file; file name matches type name
- File-scoped namespaces (`namespace Foo;` not `namespace Foo { }`)
- Namespace mirrors folder path (`CharityApi.Services` lives in `Services/`)

## ASP.NET Patterns
- Controllers are thin — no business logic, delegate to services
- Services hold all domain logic
- Models are plain data containers; no behaviour
- One controller per aggregate root

## Validation
- Use FluentValidation for all request validation
- Never validate in controllers; register validators via DI
- Return 400 with `ValidationProblemDetails` on failure

## HTTP Conventions
| Outcome | Status code |
|---------|-------------|
| Success with body | 200 OK |
| Resource created | 201 Created + Location header |
| Success, no body | 204 No Content |
| Validation failure | 400 Bad Request |
| Resource not found | 404 Not Found |
| Unexpected error | 500 Internal Server Error |

## Testing
- Framework: xunit + Moq
- Pattern: Arrange / Act / Assert, one logical assertion per test
- Test name format: `MethodName_Scenario_ExpectedBehaviour`
- Mock all external dependencies; never hit real databases in unit tests
- Integration tests use `WebApplicationFactory<Program>`
