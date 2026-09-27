# Phase 0 Research: Add Feed Subscription (MVP)

All Technical Context items had a reasonable default derivable from
`StakeholderDocuments/TechStack.md` and the project constitution; no
`NEEDS CLARIFICATION` markers remained after drafting the plan. This document records the
technology decisions made and the alternatives considered.

## Decision: Target framework — .NET 8 (LTS)

- **Decision**: Build both `RSSFeedReader.Api` and `RSSFeedReader.UI` on .NET 8.
- **Rationale**: .NET 8 is the current LTS release supporting ASP.NET Core Web API and Blazor
  WebAssembly, matches the cross-platform requirement (Windows/macOS/Linux) in
  `TechStack.md`, and gives the longest support window for a project meant to evolve through
  Extended-MVP and Post-MVP phases.
- **Alternatives considered**: .NET 9 (newer, but STS with a shorter support window; no feature in
  this MVP requires it) — rejected in favor of LTS stability.

## Decision: Backend API style — Minimal APIs

- **Decision**: Expose `POST /api/subscriptions` and `GET /api/subscriptions` using ASP.NET Core
  minimal API endpoints in `Program.cs`.
- **Rationale**: The MVP has exactly two operations and no complex model binding or filters;
  minimal APIs keep the backend small and readable, matching constitution Principle III
  (code quality/consistency) and the "keep it simple" delivery approach in `ProjectGoals.md`.
- **Alternatives considered**: MVC Controllers — rejected as unnecessary ceremony for two
  endpoints at this stage; can be introduced later if the API grows.

## Decision: In-memory storage — singleton store with a lock-protected list

- **Decision**: Register a singleton `SubscriptionStore` service holding a `List<string>` of
  subscription URLs, guarded by a simple lock for add/read operations.
- **Rationale**: FR-005 requires in-memory-only storage; a singleton service (rather than static
  state) keeps the store testable and DI-friendly. A lock is cheap insurance against races between
  concurrent requests from the Blazor client, even though MVP load is a single local user.
- **Alternatives considered**: `ConcurrentBag<string>`/`ConcurrentQueue<string>` — rejected because
  order of addition must be preserved for display (FR-002), which a bag does not guarantee.

## Decision: CORS configuration — explicit origin allow-list

- **Decision**: Configure the backend CORS policy to allow exactly the frontend's launch URLs
  (e.g., `http://localhost:5213`, `https://localhost:7025`), read from configuration rather than
  hardcoded, per constitution Principle I and the Technology & Environment Constraints section.
- **Rationale**: Wildcard origins are explicitly prohibited by the constitution; explicit origins
  keep the API from accepting cross-origin requests from unintended sources.
- **Alternatives considered**: `AllowAnyOrigin()` — rejected, violates the constitution's
  security-first principle.

## Decision: Frontend state handling — direct API calls, no client-side cache

- **Decision**: The Blazor `Subscriptions` page calls `GET /api/subscriptions` on initial render
  and re-fetches (or appends locally then confirms via the API response) after a successful
  `POST`, rather than maintaining a separate persisted client-side cache.
- **Rationale**: Keeps the backend as the single source of truth for the in-memory list (FR-002,
  FR-003) and avoids introducing state-management complexity not needed for an MVP with a single
  page.
- **Alternatives considered**: Local component-only state with no backend round-trip — rejected
  because the spec requires the list to reflect what the API/store holds, and a future
  Extended-MVP refresh action will need the same API surface.

## Decision: Template cleanup — required before/with this feature

- **Decision**: Remove `Home.razor`, `Counter.razor`, `Weather.razor` and their `NavMenu.razor`
  entries as part of implementing this feature, and give the subscriptions page the root
  `@page "/"` route.
- **Rationale**: Constitution Principle III and `TechStack.md` both flag leftover template pages
  as a documented, recurring source of ambiguous-route runtime errors; cleanup is treated as a
  prerequisite gate, not an optional nicety.
- **Alternatives considered**: Leaving demo pages on non-root routes — rejected; constitution
  requires their removal outright, not just rerouting.

**Output**: All unknowns resolved; no `NEEDS CLARIFICATION` markers remain.
