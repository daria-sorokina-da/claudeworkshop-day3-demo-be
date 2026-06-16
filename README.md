# Enchanted Stables Registry API — Claude Code Workshop (Backend Track)

A deliberately half-finished ASP.NET (.NET 8) horse-registry API used in the Anthony Nolan Claude Code workshop, Day 3 (Backend track). Over a ~4-hour hands-on lab you'll explore the code, extend it, fix a planted bug, build a reusable Claude Code toolkit, and deploy it — all by driving Claude Code yourself.

## Getting started

**Fork this repo first**, then clone your fork and work on a feature branch:

```bash
git clone https://github.com/<your-username>/claudeworkshop-day3-demo-be.git
cd claudeworkshop-day3-demo-be
git switch -c nova-none-stables-workshop

dotnet build
dotnet test                          # all 9 tests should pass
dotnet run --project src/StableApi   # http://localhost:5000/swagger
```

## The exercise

The full lab is in **[EXERCISE.md](EXERCISE.md)** — start there.

## Layout

- `src/StableApi/` — the API (Controllers / Services / Models)
- `tests/StableApi.Tests/` — xunit + Moq tests
- `infrastructure/` — Terraform for Azure
- `.claude/` — shared Claude Code settings (hooks) and code style

## Licence

See [LICENSE.md](LICENSE.md).
