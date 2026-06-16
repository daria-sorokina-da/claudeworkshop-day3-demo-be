# Hands-On Exercise — Harness Claude Code on a Real Backend

**Track:** Backend Engineers · **Duration:** ~4 hours (including a 15-minute break) · **Style:** You drive. Work individually or in pairs.

> 🍴 **Before anything else: [fork this repo](https://github.com/daria-sorokina-da/claudeworkshop-day3-demo-be)**, then clone your fork and work on a feature branch — full setup in Part 0.

---

## What you'll practice

This is a *do-it-yourself* lab, not a watch-the-facilitator demo. You'll take the half-finished **Enchanted Stables Registry API** from "I don't know this code" to "I shipped a feature, fixed a real bug, and built a reusable team toolkit that runs itself."

Every part maps back to the workshop theory:

| Part | Concepts practised |
|---|---|
| 0 — Setup | Setup check · essential commands · Git safety net |
| 1 — Onboard & harness | Prompting (precise) · Plan mode · `/init` · CLAUDE.md · `@import` · Permissions · Context management · Mermaid diagram |
| 2 — Ship & debug | Agentic loop · Spec → test workflow · Jira MCP (ticket lifecycle) · Hooks · `/rewind` · Red-test debugging · Git workflow & PR description |
| 3 — Build a team toolkit | Skills · Slash commands · Hooks (Post + PreToolUse) |
| 4 — Orchestrate sub-agents | Sub-agents (sequential + parallel) · Model selection & cost |
| Optional extras | GitHub MCP · Azure deploy (DevOps pipeline) · Headless mode · EF Core/SQLite · Plugins |

> **The mindset:** AI is an amplifier. A good decision delivered faster — or a bad one propagated across a dozen files before you notice. You stay in charge: you own *what to build*, *what trade-offs to accept*, and *what is safe to ship*. Claude materialises the intent; the quality of the intent is on you.

---

## Timeline

| Time | Part | Minutes |
|---|---|---|
| 0:00 | **Part 0** — Setup & ground rules | 10 |
| 0:10 | **Part 1** — Onboard and harness the repo | 50 |
| 1:00 | **Part 2** — Ship a feature, then fix a real bug | 65 |
| 2:05 | **Break** | 15 |
| 2:20 | **Part 3** — Build your team toolkit | 35 |
| 2:55 | **Part 4** — Orchestrate sub-agents to ship a feature | 25 |
| 3:20 | Wrap-up | 5 |
| 3:25 | **Optional extras** | remaining time |

---

## Part 0 — Setup & ground rules (10 min)

This is a standalone repo — **fork it, then work on your own fork** so your changes never touch the shared original.

1. Open **https://github.com/daria-sorokina-da/claudeworkshop-day3-demo-be** and click **Fork** (top-right).
2. Clone your fork and create a feature branch — per our [Git Standards](https://anthonynolan.atlassian.net/wiki/spaces/NPDWS/pages/532185178/Git+Standards), all work happens on a branch:

```bash
git clone https://github.com/<your-username>/claudeworkshop-day3-demo-be.git
cd claudeworkshop-day3-demo-be
git switch -c nova-none-stables-workshop     # feature branch: lowercase, card-first, hyphenated
```

3. Confirm it builds and Claude Code is ready:

```bash
dotnet build
dotnet test                          # all 9 tests should pass
dotnet run --project src/StableApi   # http://localhost:5000/swagger
claude --version                     # confirm CLI + auth
```

In real work the branch name starts with your **story** card number (e.g. `nova-1234-add-retire-endpoint`); there's no card here, so we use `nova-none`. The repo already ships a `.gitignore`, so build output stays out of your commits — just `git add . && git commit` from the repo root as you go.

If anything fails to build, fix it before moving on — raise your hand.

**Three rules of highest importance, pinned for the whole lab:**

- Never paste secrets or real patient/donor data into a prompt. Anything you put in context has left your environment, regardless of Claude's data policy.
- Never use `--dangerously-skip-permissions` on this repo.
- Always read the diff before you accept it. Confident-and-wrong is Claude's main failure mode.

**Use Git as you go — follow the [commit standard](https://anthonynolan.atlassian.net/wiki/spaces/NPDWS/pages/532185178/Git+Standards).** Commit at the end of each milestone. Every message is `type: CARD-ID: Description` — `type` is one of `feature` / `fix` / `review` / `chore` / `docs`, the card ID is capitalised (`NOVA-NONE` for this workshop), and the description is a short present-tense phrase. A clean working increment you can return to beats one giant commit at the end — and `git stash` / `git checkout .` is your undo button if a prompt goes sideways.

**Commands you'll lean on:** `/help` · `/plan` · `/rewind` · `/clear` · `/compact` · `/context` · `/usage` · `/init` · `/memory` · `Esc` (stop mid-run, keep context) · `Shift+Tab` (cycle approval modes).

**Mid-task side notes — `/btw`:** While Claude is working you can type `/btw <your note>` to inject a correction or preference without interrupting the task flow. Claude receives it as a message and adjusts — useful for small steers like *`/btw use .temp not temp`* without stopping and re-prompting from scratch.

---

## Part 1 — Onboard and harness the repo (50 min)

**Goal:** Go from cold start to a repo that hands every future session the right context automatically. You'll get oriented fast, use plan mode to look before you leap, then make that knowledge permanent in `CLAUDE.md`.

### 1.1 — Get oriented, then diagram it (10 min)

Open Claude Code (`claude`) in the repo. One precise prompt — task framing + constraints + output format — gets you a real mental model fast:

```
In 3 bullets: what this API does, how it's layered, and — tracing the
register-a-horse path from HTTP request to storage — what actually backs
the data. Read CLAUDE.md and the code first. Don't change anything.
```

✅ **Acceptance:** Without you naming a single file, Claude crosses controller → service interface → implementation and reports what backs the data — you'll find it's an in-memory collection, not a database (something you couldn't have known without exploring). That's `Glob`/`Grep`-driven discovery, not you feeding it files.

Now turn that trace into a picture you can keep:

```
Create docs/register-horse-flow.md with a Mermaid sequence diagram of the
register-a-horse path you just traced: client → controller → service → store.
```

Open the file in a Markdown preview — the diagram renders. Architecture docs for free, straight from the code.

### 1.2 — Critique the controller — no fixes yet (10 min)

```
Review HorsesController.cs and list the quality issues you find. Don't change
anything. For each issue give the method, what's wrong, and the HTTP behaviour
it should have per our code-style.md.
```

✅ **Acceptance:** Claude finds the three intentional issues — `GetById` returns 200 for a missing horse, `Create` returns 200 instead of 201 + Location, `Delete` returns 200 instead of 204 — plus the manual validation in `Create`. **Keep this list — you'll fix them in Part 2.4.**

Now persist the findings so they survive a `/clear` or a teammate's fresh session:

```
Save these issues to a file .temp/known-issues.md
```

✅ **Acceptance:** `.temp/known-issues.md` exists with each issue listed by method, what's wrong, and the required HTTP behaviour. The `.temp/` folder is gitignored — it's your personal scratch space for the session, not committed. This is also a useful pattern for real work — a review session's output becomes a durable artefact you can carry across `/clear` boundaries.

### 1.3 — Plan mode: look before you leap (10 min)

Switch to plan mode and ask for a *plan only*:

```
/plan

I want to make this codebase easier for a new engineer to find their way
around. What should we add to CLAUDE.md? Just propose it — don't write
anything yet.
```

Steer the plan in plain language before approving (e.g. *"Just stick to architecture, domain model, and test patterns — keep it short and factual."*). This separates **thinking from doing**.

✅ **Acceptance:** You see and shape the intent *before* any file changes.

### 1.4 — Make context permanent in CLAUDE.md (15 min)

Open `CLAUDE.md` — notice line 3, `@import .claude/code-style.md` (team standards loaded every session, kept separate from project facts) and the `<!-- TODO -->` stub sections.

First see what `/init` does — it scans the codebase and proposes CLAUDE.md content:

```
/init
```

Review its proposal in the diff. **Keep the `@import` line and our section headings** (Architecture, Domain Model, Test Patterns); accept the useful factual content and discard anything generic or wrong. If `/init` strays too far from the curated stub, run `git checkout CLAUDE.md` to reset and fill it deliberately instead:

```
Explore the project and fill in the Architecture, Domain Model, and
Test Patterns sections of CLAUDE.md. Keep it factual — only write things
you've actually confirmed by reading the code. For Test Patterns, look at
HorsesControllerTests.cs and capture the Arrange/Act/Assert + Moq pattern
and the MethodName_Scenario_ExpectedBehaviour naming.
```

Review the diff and correct anything imprecise. Then **prove it loads**:

```
/clear

What HTTP status code should a successful DELETE return, per our code style?
```

✅ **Acceptance:** After `/clear` wipes the conversation, Claude still answers **204 No Content** — because `CLAUDE.md` (and its `@import`) reload automatically. You wrote it once; every future session and teammate benefits.

### 1.5 — Permissions are a hard rule, not advice (5 min)

This repo already denies edits to `appsettings.Production.json` (see `.claude/settings.json`). Test it:

```
Add a comment line to src/StableApi/appsettings.Production.json.
```

✅ **Acceptance:** Claude is blocked by the `deny` rule — it never gets the chance, regardless of what you ask. Telling Claude "don't touch prod" in a prompt is advice; `deny` in settings is enforcement by the harness.

> **Context check:** Run `/usage`. If you're climbing past ~60%, `/compact` now while recall is still clean before starting Part 2.

> **Commit checkpoint:** `git add . && git commit -m "docs: NOVA-NONE: Document architecture and test patterns in CLAUDE.md"`

---

## Part 2 — Ship a feature, then fix a real bug (65 min)

**Goal:** Run the full agentic loop twice — once to *build* (spec → model → service → controller → validation → tests), once to *debug* (inspect → red test → fix → green). The repo's PostToolUse hooks auto-format on every `src` edit and auto-run tests on every `tests` edit, so verification is free.

### 2.1 — Plan the feature first (10 min)

> **Optional — start from the ticket (Jira MCP).** In real work the spec comes from a story, not a prompt. If the Atlassian MCP is set up, pull the retire endpoint's requirements from Jira instead of reading them below.
> *Setup (facilitator):* a throwaway workshop story, e.g. `NOVA-####`, with acceptance criteria, and the Atlassian MCP added to `.mcp.json` — `{"mcpServers":{"atlassian":{"type":"sse","url":"https://mcp.atlassian.com/v1/sse"}}}` — authenticated via `/mcp`. If you use a real ticket, put its number in your branch and commits (`feature: NOVA-1234: …`) instead of `NOVA-NONE`.
> ```
> Read Jira ticket NOVA-#### with the Atlassian MCP and summarise its
> acceptance criteria — we'll build against these.
> ```
> No Jira set up? Just use the spec in the prompt below.

```
/plan

I need a POST /api/horses/{id}/retire endpoint. The body carries a Reason
string. It returns 200 with the updated horse, or 404 if the horse does not
exist. Walk me through the plan — list every file you'll create or modify —
before writing any code.
```

Approve once the plan is scoped and sensible.

### 2.2 — Build it (20 min)

You don't have to dictate it layer by layer — hand Claude the whole slice from the plan and review the diffs as they land:

```
Implement the retire endpoint from the plan: a RetireRequest DTO, a Retire
method on IHorseService/HorseService (mark the horse inactive, store the
reason), the controller action (200 with the updated horse, or 404), and a
FluentValidation RetireRequestValidator (Reason 5–200 chars) registered in
Program.cs.
```

Watch the **format hook** fire after each `src` edit, and read each diff before accepting — small, reviewable changes even though the prompt was one line.

> **Try `/rewind`:** don't like an edit Claude just made? Run `/rewind` to roll back the last action — conversation *and* file changes — with no Git needed. Undo one step, then re-prompt it more precisely. (Git checkpoints are for milestones; `/rewind` is for in-the-moment undo.)

### 2.3 — Tests close the loop (10 min)

```
Add unit tests for the retire endpoint in HorsesControllerTests.cs. Cover:
valid request returns 200, reason shorter than 5 chars returns 400, and a
non-existent horse returns 404. Follow our existing test patterns.
```

✅ **Acceptance:** When the test file saves, the **PostToolUse test hook** runs `dotnet test` automatically. If anything is red, let Claude self-correct from the hook output — you don't switch to the terminal.

> **Commit checkpoint:** green tests = a working increment. `git add . && git commit -m "feature: NOVA-NONE: Add retire endpoint with validation and tests"`

### 2.4 — Refactor the three issues, one at a time (15 min)

Open `.temp/known-issues.md` from Part 1.2 and fix the issues **individually**, verifying each:

```
Earlier we found three quality issues in HorsesController. Fix them one at a
time, and after each fix update or add the matching test.
1) GetById should return 404 when the horse does not exist.
2) Create should return 201 Created with a Location header to the new horse.
3) Delete should return 204 No Content.
Then replace the manual validation in Create with a CreateHorseRequestValidator
(Name >= 2 chars, OwnerEmail must contain '@'), registered via DI.
```

✅ **Acceptance:** All tests green after each step. Identify → confirm → change one thing → verify.

> **Commit checkpoint:** `git add . && git commit -m "fix: NOVA-NONE: Correct HorsesController status codes and use FluentValidation"`

### 2.5 — The debugging loop on a real bug (10 min)

There's a genuine pagination bug. Surface it without telling Claude the answer:

```
Users report GET /api/horses?page=1&pageSize=2 returns horses 3 and 4
instead of 1 and 2. Help me find the cause. Don't fix it yet — show me the
exact line where the spec and the code disagree.
```

Claude should trace to `HorseService.GetPaged` and spot that the doc comment says **1-based** but the code does `Skip(page * pageSize)` (0-based). Then:

```
Before fixing, write a failing test in a new HorseServiceTests.cs proving
that page 1, pageSize 2 returns the first two horses. Confirm it fails.
```

```
Now fix the bug so the test passes.
```

✅ **Acceptance:** The fix is `Skip((page - 1) * pageSize)`; the red test goes green; all other tests still pass. **Red first, then green** — the failing test is the proof of the bug.

> **Commit, then draft the PR — follow the [Git Standards](https://anthonynolan.atlassian.net/wiki/spaces/NPDWS/pages/532185178/Git+Standards):**
> ```
> Review my current git diff — flag anything accidental or any missing tests,
> but don't change any files. Then write a commit message in our format
> (type: CARD-ID: present-tense description; use NOVA-NONE here) and commit.
> ```
> Then draft the pull request you'd open against `develop`:
> ```
> Draft a PR description for this branch with sections: Summary, What changed,
> How tested, Risks / follow-ups. Don't modify any files.
> ```
> In real work you'd push the feature branch and open the PR in GitHub (reviewed via reviewable.io, merged with **Rebase and Delete** to keep history linear). We're not pushing to the shared repo during the lab — drafting the description is the exercise.

> **Optional — close the ticket loop (Jira MCP).** If you read the spec from Jira in 2.1, now move the story along the way you would after raising a PR (use a throwaway ticket — these prompts actually change it):
> ```
> Transition NOVA-#### to "Ready for Review" using the Atlassian MCP.
> ```
> ```
> Add a comment to NOVA-#### summarising the implementation: the endpoint
> added, validation rules, tests, and a link to the PR.
> ```
> If the transition errors, the status name must match a real workflow transition — ask Claude to list the available ones first. Verify the status and comment in Jira.

---

## ☕ Break — 15 minutes

---

## Part 3 — Build your team toolkit (35 min)

**This is where you stop typing every prompt and make the *repo* carry your workflow** — a Skill, a slash command, and a hook: the shared "what lives in the repo" context every teammate inherits on clone. You'll put the toolkit to work in Part 4.

### 3.1 — Get a Skill: draft your own, then install one (15 min)

A Skill is on-demand context Claude loads *only when relevant* — more token-efficient than `CLAUDE.md`, which loads every session. You'll get one both ways.

**Draft your own from what you just did.** You built the retire endpoint by hand in Part 2 — codify that recipe so Claude repeats it. Have Claude write it for you:

```
You just helped me build the retire endpoint. Capture the repeatable recipe
as a skill at .claude/skills/new-aggregate.md: the order to scaffold a new
entity (model → request DTOs → service → thin controller with correct HTTP
codes → FluentValidation registered in Program.cs → xunit/Moq tests), all
following our code-style.md. Keep it tight, and add a description line so it
auto-loads when I add a new entity.
```

Review the draft and tighten anything vague. ✅ **Acceptance:** Ask *"How should I add a new entity here?"* and confirm it invokes the skill (or trigger `/new-aggregate`). The guidance comes from the skill, not from you re-explaining it each time.

**Install a published one.** You don't have to write every skill — they install like packages. Add a marketplace and install one ([plugin docs](https://code.claude.com/docs/en/discover-plugins); Anthropic's catalogue is [anthropics/skills](https://github.com/anthropics/skills), with community marketplaces too):

```
/plugin marketplace add anthropics/skills
/plugin            # browse, then install one — e.g. pdf
```

Then put it to work — e.g. have the `pdf` skill turn your API into a shareable artifact:

```
Using the pdf skill, generate a one-page PDF API reference from the
controllers: endpoints, methods, and status codes.
```

✅ **Acceptance:** a published skill you didn't write is installed and produces something real. (Needs network; if the marketplace path differs, browse `/plugin`.)

### 3.2 — Author a slash command (10 min)

A slash command is a workflow *you* trigger deliberately. Rule of thumb: *if you run the same prompt twice, codify it.* Create `.claude/commands/review.md`:

```markdown
Review the current git diff against our team standards:
- Controllers stay thin; logic lives in services
- FluentValidation used; no manual validation in controllers
- Correct HTTP codes (201+Location, 204, 404)
- Tests exist for new logic and follow our naming format
- No secrets, no unrelated changes
- Commit messages follow our standard (type: CARD-ID: description)
Report issues as a checklist. Do not modify any files.
```

✅ **Acceptance:** `/review` runs your standardised review on demand.

### 3.3 — Add a verification hook (10 min)

The repo already auto-formats `src` edits and auto-tests `tests` edits. You'll add two things to `.claude/settings.json`: a `deny` rule, and a new hook that builds the project after every source edit so a compile error never sneaks past you.

**1. Add the deny rule.** Open `.claude/settings.json`. Inside the existing `"permissions": { "deny": [ ... ] }` array, add this line at the top (keep the comma):

```json
"Bash(git push *)",
```

**2. Add the build-check hook.** Inside the existing `"hooks": { "PostToolUse": [ ... ] }` array, paste this as a new object (add a comma after the previous `}` so the array stays valid):

```json
{
  "matcher": "Edit(src/.*\\.cs)",
  "hooks": [
    {
      "type": "command",
      "command": "dotnet build src/StableApi/StableApi.csproj --nologo -v q 2>&1 | tail -5 || true"
    }
  ]
}
```

This runs `dotnet build` after every `src` edit — if your change doesn't compile, you see it immediately instead of three steps later.

**3. Test it.** Restart Claude Code (so it reloads settings), then ask Claude to make any small `src` edit and watch the build fire in the hook output. Then confirm the deny works:

```
Push the current branch to origin.
```

✅ **Acceptance:** The build hook runs after a source edit, and the `git push` request is blocked outright. You can explain the difference: a **permission** stops the action before it ever happens; a **hook** runs a check around it.

**Optional — block secrets *before* they're written (PreToolUse).** A `PostToolUse` hook runs *after* a change; a `PreToolUse` hook runs *before* and can veto it. Create `.claude/hooks/block-secrets.mjs`:

```js
const input = JSON.parse(await new Response(process.stdin).text());
const { content = "" } = input.tool_input ?? {};
const secretPatterns = [/AKIA[0-9A-Z]{16}/, /sk-[A-Za-z0-9]{20,}/, /-----BEGIN (RSA |EC )?PRIVATE KEY/];
if (secretPatterns.some((re) => re.test(content))) {
  console.error("Blocked: looks like a hard-coded secret — use configuration instead.");
  process.exit(2); // exit 2 = block AND show this message to Claude
}
process.exit(0);
```

Add a `PreToolUse` block alongside the existing `PostToolUse` in `hooks`:

```json
"PreToolUse": [
  {
    "matcher": "Write|Edit",
    "hooks": [ { "type": "command", "command": "node .claude/hooks/block-secrets.mjs" } ]
  }
]
```

Test it: ask Claude to write a fake AWS key (`AKIAIOSFODNN7EXAMPLE`) into a source file. The hook blocks the write, and because exit code `2` feeds the message back, Claude self-corrects instead of just failing — exactly the data-leak guardrail from the theory session.

---

## Part 4 — Orchestrate sub-agents to ship a feature (25 min)

Now put the toolkit to work. You'll deliver a real feature through a team of sub-agents — **Races with entrants: full CRUD plus real business rules.**

```
A Race has: Id, Name, TrackName, StartDate, IsActive.
- Race CRUD: model + Create/Update DTOs, IRaceService + RaceService (in-memory),
  RacesController (GET all + paged, GET by id, POST, PUT, DELETE).
- Entrants: POST /api/races/{id}/enter (body { HorseId }), GET /api/races/{id}/entrants.
- Rules, each a clear 4xx + message: unknown horse -> 404; a retired (inactive)
  horse can't enter; the same horse can't enter twice; no entries after StartDate.
- FluentValidation for the Race fields; the entry rules live in the service.
- Unit tests for the controller actions and every rule.
```

> **Pick the right model — and watch the spend.** This part mixes cheap repetitive work with harder reasoning, so use `/model` to switch: keep most work on Sonnet, reach for higher thinking effort (or Opus) on the multi-step orchestration, and note the `test-runner` agent below is pinned to Haiku because writing and running tests is high-volume and cheap. Run `/cost` whenever you want to see what the session has spent.

Sub-agents are independent work streams with their own context and scoped tools. Create three under `.claude/agents/`:

`.claude/agents/implementer.md`

```markdown
---
name: implementer
description: Implements a planned feature across model, service, and controller layers.
tools: Read, Edit, Write
---
You implement features following the new-aggregate skill and code-style.md.
Make minimal, layered changes. Do not write tests — that is the test-runner's job.
```

`.claude/agents/test-runner.md`

```markdown
---
name: test-runner
description: Use PROACTIVELY after code changes to write and run tests and fix failures.
tools: Bash, Read, Edit, Write
model: claude-haiku-4-5
---
You are a test automation expert. Write xunit+Moq tests for new logic,
run `dotnet test`, and fix failures while preserving test intent. Report what was fixed.
```

`.claude/agents/reviewer.md`

```markdown
---
name: reviewer
description: Read-only reviewer. Checks a diff against team standards. Never edits.
tools: Read, Bash
---
Review changes against code-style.md. Output a checklist of issues. Do not modify files.
```

Now orchestrate them from the main session (orchestration lives in *your* prompt, not the agent files):

```
/plan

Add Races with entrants. A Race has: Id, Name, TrackName, StartDate, IsActive.
- Race CRUD: model + Create/Update DTOs, IRaceService + RaceService (in-memory),
  RacesController (GET all + paged, GET by id, POST, PUT, DELETE).
- Entrants: POST /api/races/{id}/enter (body { HorseId }), GET /api/races/{id}/entrants.
- Enforce the rules in the service, each with a clear 4xx + message:
  unknown horse -> 404; a retired (inactive) horse cannot enter; the same horse
  cannot enter twice; no entries once StartDate has passed.
- FluentValidation for the Race fields; unit tests for the controller and every rule.

Plan the work first, then build it in this order:
- use the implementer agent for the model, DTOs, service, and controller;
- then the test-runner agent to write and run all tests (cover each rule);
- then the reviewer agent to check the final diff.
List the plan before touching anything.
```

✅ **Acceptance:** Race CRUD ships, all tests pass, and the reviewer returns a clean (or actionable) checklist. You used **sequential orchestration** — implementer → test-runner → reviewer — each with a scoped toolset.

**Now feel the difference — fan out in parallel.** Sequential is right when each step needs the previous one's output. When tasks are independent, run them at once. Launch three reviewers in parallel over the Race changes, each on a different concern, then aggregate:

```
Review the Race changes with three reviewer agents running in parallel, each
focused on one concern: (1) security, (2) performance/efficiency, (3) naming
and code-style conventions. Run them concurrently, then merge their findings
into one prioritised checklist.
```

Three agents work at once, each in its own context window — far faster than one agent doing three passes, and the same pattern fans out *writes* too (one implementer per service in a monorepo) whenever the tasks touch independent files.

> **Commit checkpoint:** capture both the toolkit and the feature it produced. `git add . && git commit -m "feature: NOVA-NONE: Add races with entrant rules via skill/command/agent toolkit"`

---

## Wrap-up

You took an unfamiliar repo and, in one sitting: onboarded with precise prompting, made the context permanent, shipped a feature and fixed a real bug through the agentic loop, and built a self-running team toolkit that then shipped a second feature for you. The thing to take away: **the loop is the same whether the task took 30 seconds or 3 hours — you don't close it until it's green, and you stay in charge of every decision.**

Before you stop: commit any loose work on your `nova-none-stables-workshop` branch (`type: CARD-ID: Description`), run `/cost` to see what the whole session spent, and skim `git log --oneline` — that clean, milestone-by-milestone history is what a reviewer would see.

---

## Optional extras — if you have time

You've done the core. These are the high-ceiling add-ons — pick whatever appeals; none depends on the others. The first three need a little setup.

### Action PR review comments with the GitHub MCP — *needs setup*

MCP is "USB for AI tools": one standard, any tool. The real payoff is closing the review loop — your PR comes back with comments, and instead of copy-pasting each one, Claude reads them straight from GitHub and fixes them.

> **Setup needed before this step (facilitator):** a GitHub repo holding the Stables code with **an open PR that already carries a few review comments** (e.g. *"GetById should return 404 for a missing horse"*, *"replace the manual validation with FluentValidation"*). Each participant needs a **GitHub personal access token** (repo scope) exported as `GITHUB_TOKEN`. Tokens go in `~/.claude.json` or your shell env — never committed.

Add a project-level `.mcp.json` at the repo root with the GitHub server (use the `${GITHUB_TOKEN}` placeholder — never paste the token itself):

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"],
      "env": { "GITHUB_TOKEN": "${GITHUB_TOKEN}" }
    }
  }
}
```

Restart Claude Code and run `/mcp` to confirm `github` connected. Then have Claude pull the review comments and address them — review every diff before accepting:

```
Read the open review comments on PR #<number> in <owner>/<repo> using the
GitHub MCP. List each comment, then fix the code it refers to — one commit
per comment, message format: fix: NOVA-NONE: <what you changed>.
Don't push; I'll review the diffs first.
```

Optionally, have Claude reply to each thread via MCP so the PR shows what was done:

```
Reply to each of those review comments on the PR summarising the fix you made.
```

✅ **Acceptance:** `/mcp` lists the `github` server, Claude reads the actual PR comments, and each one maps to a local fix you reviewed. The GitHub MCP tools looked identical to built-in tools from Claude's side — same tool-call loop. **No token?** Use the zero-credential filesystem server instead to see the wiring (`"command": "npx", "args": ["-y", "@modelcontextprotocol/server-filesystem", "."]`), then ask Claude to use one of its tools.

### Deploy to Azure App Service via an Azure DevOps pipeline — *needs setup*

Generate the infrastructure, then ship the app for real through a pipeline — the satisfying finish: your API live on the internet.

> **Setup needed (facilitator):** an Azure subscription (a shared sandbox is fine), an Azure DevOps project holding this repo with a service connection to the subscription, and the [Azure DevOps MCP](https://learn.microsoft.com/en-us/azure/devops/mcp-server/mcp-server-overview) connected (local `npx` or the remote endpoint — see the docs). Use the cheapest tier and **`terraform destroy` afterwards** so nothing keeps billing.

First the infrastructure — App Service only, no database:

```
Read the requirements comment in infrastructure/main.tf and generate the
Azure resources with variables from variables.tf: resource group, Linux
App Service plan, and the .NET 8 web app. Add a random suffix to globally
unique names. Run terraform init and terraform plan, and show me the plan.
```

Review the HCL — *you* own the security sign-off — then apply it (facilitator, against the sandbox subscription). Now the pipeline. Have Claude write the build-and-deploy definition:

```
Create azure-pipelines.yml that builds and publishes the .NET 8 API and
deploys it to our App Service with the AzureWebApp task, using our service
connection. Trigger on the main branch.
```

Then use the Azure DevOps MCP to run it and report back:

```
Using the Azure DevOps MCP, queue a run of the pipeline, wait for it, and
report the result and the live app URL. If it fails, summarise why.
```

✅ **Acceptance:** the pipeline runs in Azure DevOps and the API answers on its public URL — try `GET /api/horses` over the internet. You produced infra, a pipeline, and a live deployment without hand-clicking the portal. (Run `terraform destroy` when you're done.)

### Run a headless CI gate

Close the loop with **no human in it**. From a normal terminal:

```bash
claude -p "Run the full test suite and summarise any failures." \
  --allowedTools Bash,Read --max-turns 5
echo "exit code: $?"
```

✅ **Acceptance:** Claude runs headless, prints a summary, and exits — `0` on pass, non-zero on failure. You **scoped `--allowedTools` to `Bash,Read`**: a read-only task needs no `Write`. Minimum surface = minimum blast radius — exactly how you'd wire Claude into a CI step.

And a handful more, biggest first:

- **Make it persist for real — migrate to EF Core + SQLite.** Everything currently lives in an in-memory `Dictionary`, so data vanishes on restart. Do the whole multi-file change in one agentic session — plan first, then let the hooks verify each step:
  ```
  /plan

  Migrate the Stables API from its in-memory store to EF Core with SQLite.
  Plan the full change first, then implement it:
  - add the EF Core + SQLite NuGet packages;
  - create a StableDbContext with a Horses DbSet;
  - turn Horse into an entity and replace the Dictionary in HorseService with
    DbContext queries (keep IHorseService unchanged so the controllers don't care);
  - register the DbContext in Program.cs against a SQLite connection string,
    and apply migrations on startup;
  - add the initial migration;
  - keep every test green — use the EF Core in-memory provider for service tests.
  Build and run the tests after each step.
  ```
  **The payoff:** once it's green, `dotnet run`, POST a new horse, stop the app, restart it — the horse is still there. Same `IHorseService` contract, now backed by a real database, and the controller layer never changed a line.
- **Close the loop against the *running* API — not just unit tests.** Have Claude boot the server and exercise the real endpoints over HTTP, reading the actual responses and iterating:
  ```
  Start the API in the background with `dotnet run`, then exercise it over HTTP
  with curl (not unit tests):
  - GET /api/horses?page=1&pageSize=2 and confirm you get the first two horses;
  - POST a new horse, then GET it back by the id you got;
  - GET a non-existent id and confirm it returns 404.
  Show me the actual requests and JSON responses, then stop the server.
  ```
  **The payoff:** this is the agentic loop closing against reality — Claude starts a server it then calls, reads the live JSON, and confirms the behaviour end-to-end. Watching it spin up the API, curl its own endpoints, and tear it down feels qualitatively different from green unit tests.
- Try these prompting patterns on the repo and compare what each gives you:
  - **Precise vs. vague** — run `Tell me about this project.`, then a tightly scoped version, and notice the gap:
    ```
    In 3 bullets: what this API does, how it's layered, and how it's tested.
    Read CLAUDE.md and the code first. Don't change anything.
    ```
  - **Few-shot** — show the format by example and Claude matches it:
    ```
    List the endpoints in HorsesController. Use exactly this format, like the example:
    GET /api/horses — lists horses, paged (200)
    Now do the same for every other endpoint.
    ```
  - **Chain-of-thought** — make it reason in numbered steps, no code:
    ```
    I'm thinking about adding a "breed statistics" endpoint. Reason it through
    step by step: Step 1 which layer owns it and why; Step 2 what it depends on;
    Step 3 the route and method signature; Step 4 your recommendation.
    ```
  - **XML tags** — separate instruction from output format so neither bleeds into the other:
    ```
    <instruction>
    Draft a one-paragraph summary of the Horse domain model for a new engineer.
    Don't change any files.
    </instruction>
    <output>
    Plain prose, no more than 4 sentences.
    </output>
    ```
- Add a staging deployment slot to the App Service, and move a sensitive app setting into Azure Key Vault with a Key Vault reference — then review the security implications yourself.
- Write a `/release` command. Create `.claude/commands/release.md` with:
  ```markdown
  Pre-release checklist — report pass/fail for each, don't change anything:
  - All tests pass (run dotnet test)
  - No stray Console.WriteLine left in src/
  - No open TODO/FIXME comments in src/
  - CLAUDE.md is up to date with any new endpoints
  ```
- Add a `Stop` hook that pings you when Claude finishes. In `.claude/settings.json`, add to `hooks`:
  ```json
  "Stop": [
    { "hooks": [ { "type": "command", "command": "echo \"✅ Claude finished — review the diff before committing.\"" } ] }
  ]
  ```
- Bundle your toolkit (skill + command + hook + agents) into an installable plugin and have a partner install it on a fresh clone — the onboarding test: *"Orient me in this codebase and help me pick up the first ticket."*

---

## Sources

- Git branch naming, merge flow, and commit-message format: [Git Standards — New Platform Development (Confluence)](https://anthonynolan.atlassian.net/wiki/spaces/NPDWS/pages/532185178/Git+Standards)
- Installing published skills/plugins: [Discover and install prebuilt plugins — Claude Code docs](https://code.claude.com/docs/en/discover-plugins)
- Anthropic published skills catalogue: [anthropics/skills (GitHub)](https://github.com/anthropics/skills)
- Azure DevOps MCP server (work items, repos, pipelines): [Azure DevOps MCP — Microsoft Learn](https://learn.microsoft.com/en-us/azure/devops/mcp-server/mcp-server-overview)
