# Implementation Plan: Add Feed Subscription (MVP)

**Branch**: `001-add-feed-subscription` | **Date**: 2026-09-27 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-add-feed-subscription/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Deliver the MVP subscription-management slice of the RSS feed reader: a user pastes a feed URL
into an input field, submits it, and immediately sees it appear in a displayed list of
subscriptions. Storage is in-memory only for this feature; no feed fetching, parsing, validation,
or removal is included (all deferred to later phases per the stakeholder documents).

## Technical Context

**Language/Version**: C# 12 / .NET 8 (LTS) — ASP.NET Core Web API backend, Blazor WebAssembly
frontend, per `StakeholderDocuments/TechStack.md`.

**Primary Dependencies**: ASP.NET Core minimal APIs (backend); Blazor WebAssembly component model
+ `HttpClient` (frontend). No external NuGet packages are required for this feature — no feed
parsing or HTTP-fetch libraries are needed yet.

**Storage**: In-memory only — a single backend-side list of subscription URLs held for the
lifetime of the running process (per FR-005). No database.

**Testing**: Manual verification via the local development checklist (documented in
`quickstart.md`), per constitution Principle V. Automated tests (xUnit) are not mandated for this
feature since no network or parsing logic is introduced yet.

**Target Platform**: Local development, cross-platform (Windows/macOS/Linux) — Kestrel-hosted API
plus a browser-hosted Blazor WebAssembly client.

**Project Type**: Web application (frontend + backend) — Option 2 structure below.

**Performance Goals**: Not applicable — single local user, single session; no throughput or
latency targets for this feature.

**Constraints**: Subscriptions MUST NOT persist across restarts (FR-005); no feed
fetching/parsing/validation (FR-006, FR-004); backend CORS MUST explicitly allow the frontend's
origin (constitution Principle I / Technology & Environment Constraints).

**Scale/Scope**: Single user, single session, a small in-memory list (expected tens of entries at
most for a demo) — no scale requirements.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Applicability to this feature | Status |
|-----------|-------------------------------|--------|
| I. Security-First by Default | No remote content is fetched or rendered in this feature, so XSS sanitization is not yet triggered. CORS MUST still explicitly list the frontend origin (no wildcards). No secrets are involved. | PASS |
| II. Maintainable Layered Architecture | Backend (API + in-memory store) and frontend (Blazor UI) stay separated; no logic duplication. Config (ports, `ApiBaseUrl`, CORS origins) stays externalized. | PASS |
| III. Code Quality & Consistency | Template demo pages (`Home.razor`, `Counter.razor`, `Weather.razor`) MUST be removed and replaced with a single-route subscriptions page before/while implementing this feature (tracked as a task). | PASS (enforced via task) |
| IV. Incremental, Scope-Disciplined Delivery | Feature is scoped to add + display only, matching FR-004/005/006; no persistence, polling, or parsing introduced. | PASS |
| V. Test-Informed Verification | The documented local dev checklist (backend/frontend run, `ApiBaseUrl` correct, CORS allows frontend origin, console free of errors) is captured in `quickstart.md` as the verification gate. | PASS |

No violations identified; Complexity Tracking table is not needed.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
backend/
└── RSSFeedReader.Api/
    ├── Program.cs               # Minimal API endpoints, CORS policy
    ├── Models/
    │   └── Subscription.cs      # Subscription record (Url)
    ├── Services/
    │   └── SubscriptionStore.cs # In-memory singleton store
    └── Properties/
        └── launchSettings.json  # Backend port

frontend/
└── RSSFeedReader.UI/
    ├── Pages/
    │   └── Subscriptions.razor  # @page "/" — input + add button + list
    ├── Layout/
    │   └── NavMenu.razor        # Updated to remove demo links
    ├── Properties/
    │   └── launchSettings.json  # Frontend port
    └── wwwroot/
        └── appsettings.json     # ApiBaseUrl pointing at backend port
```

**Structure Decision**: Option 2 (web application: separate backend API + frontend UI), matching
`StakeholderDocuments/TechStack.md`'s ASP.NET Core Web API + Blazor WebAssembly architecture.
`RSSFeedReader.Api` owns the in-memory subscription store and exposes it via minimal API
endpoints; `RSSFeedReader.UI` owns the subscription form/list page and calls the API through
`HttpClient`.

## Complexity Tracking

*No Constitution Check violations were identified for this feature; this section is intentionally
left empty.*
