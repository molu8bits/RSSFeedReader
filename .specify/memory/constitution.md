<!--
Sync Impact Report
==================
Version change: (none) → 1.0.0 (initial ratification)
Modified principles: N/A — initial adoption
Added sections:
  - Core Principles: I. Security-First by Default, II. Maintainable Layered Architecture,
    III. Code Quality & Consistency, IV. Incremental Scope-Disciplined Delivery,
    V. Test-Informed Verification
  - Technology & Environment Constraints
  - Development Workflow & Quality Gates
  - Governance
Removed sections: none
Follow-up TODOs: none
-->
# RSSFeedReader Constitution

## Core Principles

### I. Security-First by Default (NON-NEGOTIABLE)
Secrets, connection strings, and API keys MUST NOT be committed to source control; use
user-secrets, environment variables, or configuration providers instead. CORS policies MUST
explicitly enumerate allowed origins (the actual frontend launch ports); wildcard origins are
prohibited. Any feature that renders remote or user-supplied content (feed titles, item bodies,
HTML) MUST sanitize that content before rendering to prevent XSS. Code that fetches external
resources (feed URLs, HTTP clients) MUST set explicit timeouts and MUST NOT trust unvalidated
external input for control flow or file/path operations.
Rationale: the project already anticipates fetching third-party feeds and rendering their
content, both classic OWASP risk surfaces (XSS, SSRF, injection); treating security as a
retrofit invites vulnerabilities that are costly to remove later.

### II. Maintainable Layered Architecture
The solution MUST maintain a clear separation between the backend (ASP.NET Core Web API) and
the frontend (Blazor WebAssembly); business and data logic MUST NOT be duplicated across both
projects. Each delivery phase (MVP → Extended-MVP → Post-MVP) MUST be additive: existing public
API contracts and UI routes MUST NOT be broken without a documented migration note. Configuration
values (API base URL, ports, CORS origins) MUST be externalized to `appsettings*.json` /
`launchSettings.json`, never hardcoded in source.
Rationale: TechStack.md commits to an incremental, future-ready architecture; hardcoded values
and logic duplication break that promise and increase coupling between layers.

### III. Code Quality & Consistency
C# code MUST follow standard .NET naming and formatting conventions. Every new Razor page MUST
declare a unique `@page` route, and template demo pages (`Home.razor`, `Counter.razor`,
`Weather.razor`) plus their navigation entries MUST be removed before any MVP feature work
begins. Dead code, commented-out blocks, and unused template artifacts MUST be removed before a
change is considered complete.
Rationale: TechStack.md documents a real, recurring failure mode (ambiguous routes from leftover
template pages) that only surfaces at runtime; codifying cleanup as a gate prevents that
class of costly, late-discovered bug.

### IV. Incremental, Scope-Disciplined Delivery
Work MUST implement only the current phase's defined scope (MVP, Extended-MVP, or Post-MVP) as
described in `StakeholderDocuments/AppFeatures.md`; features belonging to a later phase MUST NOT
be pulled forward without an explicit, documented scope change. The MVP phase MUST NOT introduce
persistence, background polling, or feed parsing/fetching — these are deferred by design. Each
phase's "definition of done" (as stated in `StakeholderDocuments/ProjectGoals.md`) MUST be
verified before starting the next phase.
Rationale: the stakeholder documents define a strict phased scope specifically to keep delivery
fast; scope creep would contradict the documented minimal-first delivery approach.

### V. Test-Informed Verification
Before a phase is declared complete, the documented local development checklist (backend runs
without errors, frontend runs without errors, `ApiBaseUrl` configuration is correct, CORS allows
the frontend origin, browser console is free of connection errors) MUST be executed and pass.
Once network or parsing logic is introduced (Extended-MVP onward), automated tests (xUnit) MUST
cover feed fetching/parsing and error-handling paths. Manual verification steps MUST be
reproducible via documented commands, not performed ad hoc.
Rationale: the MVP intentionally has no automated tests yet, but verification discipline must
still exist to catch regressions such as the routing conflicts already documented in TechStack.md.

## Technology & Environment Constraints

- Backend: ASP.NET Core Web API, cross-platform (Windows, macOS, Linux).
- Frontend: Blazor WebAssembly.
- Storage: the MVP phase uses in-memory storage only; when persistence is introduced it MUST
  use EF Core + SQLite, per `StakeholderDocuments/TechStack.md`.
- Ports and CORS: backend and frontend ports MUST stay synchronized across
  `backend/RSSFeedReader.Api/Properties/launchSettings.json`,
  `frontend/RSSFeedReader.UI/Properties/launchSettings.json`, and
  `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` (`ApiBaseUrl`); the backend CORS policy
  MUST list the exact frontend origin(s) in use.
- Feed parsing, when introduced, MUST use `System.ServiceModel.Syndication`. HTML rendering of
  feed content, when introduced, MUST go through a sanitization library.

## Development Workflow & Quality Gates

- Before implementing any UI feature, template page cleanup MUST be verified complete via
  `dotnet clean`, `dotnet build`, and a manual check that only intended routes exist.
- Every feature change MUST state which phase (MVP / Extended-MVP / Post-MVP) it belongs to and
  MUST NOT include functionality out of that phase's scope.
- Configuration changes (ports, CORS, API URLs) MUST be validated by running backend and
  frontend together and confirming the browser console shows no connection errors before the
  change is marked done.

## Governance

This constitution supersedes ad hoc practices; specs, plans, and tasks MUST be checked for
compliance with these principles before implementation begins. Amendments require: a documented
rationale, a version bump following semantic versioning (MAJOR: backward-incompatible principle
removal or redefinition; MINOR: a new principle or materially expanded guidance; PATCH:
clarification or wording fixes), and an updated Sync Impact Report. Compliance reviews occur at
each phase gate (MVP → Extended-MVP → Post-MVP transitions); any deviation MUST be justified in
the relevant plan's Complexity Tracking section.

**Version**: 1.0.0 | **Ratified**: 2026-09-27 | **Last Amended**: 2026-09-27
