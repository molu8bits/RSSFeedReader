# Tasks: Add Feed Subscription (MVP)

**Input**: Design documents from `/specs/001-add-feed-subscription/`

**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/subscriptions-api.md](./contracts/subscriptions-api.md), [quickstart.md](./quickstart.md)

**Tests**: Not included as separate task items — the specification does not request automated
tests for this feature (constitution Principle V defers automated tests to Extended-MVP).
Verification is manual, via the checklist and scenarios in [quickstart.md](./quickstart.md)
(Phase 5, T018–T019).

**Organization**: Tasks are grouped by user story to enable independent implementation and
testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2)
- File paths are relative to the repository root

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create the `backend/RSSFeedReader.Api` (ASP.NET Core Web API) and
      `frontend/RSSFeedReader.UI` (Blazor WebAssembly) projects targeting .NET 8, per the
      project structure in [plan.md](./plan.md)
- [ ] T002 [P] Configure the backend port in
      `backend/RSSFeedReader.Api/Properties/launchSettings.json`
- [ ] T003 [P] Configure the frontend port in
      `frontend/RSSFeedReader.UI/Properties/launchSettings.json`
- [ ] T004 [P] Set `ApiBaseUrl` in `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` to point
      at the backend port configured in T002

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Prerequisites that MUST complete before any user story work begins

- [ ] T005 [P] Remove the template demo pages `Home.razor`, `Counter.razor`, and
      `Weather.razor` from `frontend/RSSFeedReader.UI/Pages/`, and remove their corresponding
      links from `frontend/RSSFeedReader.UI/Layout/NavMenu.razor` (constitution Principle III)
- [ ] T006 Verify template cleanup by running `dotnet clean` and `dotnet build` on
      `frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj` and confirming no ambiguous-route
      errors, per the Setup steps in [quickstart.md](./quickstart.md)
- [ ] T007 [P] Create the `Subscription` model in
      `backend/RSSFeedReader.Api/Models/Subscription.cs` with a single required `Url` field of
      type `string` (data-model.md: "`Url` MUST NOT be empty or whitespace-only before being
      added to the store")
- [ ] T008 Create the `SubscriptionStore` singleton service in
      `backend/RSSFeedReader.Api/Services/SubscriptionStore.cs` holding a lock-protected,
      insertion-ordered list of `Subscription` records (research.md: in-memory storage decision)
- [ ] T009 Register `SubscriptionStore` as a singleton and configure an explicit CORS origin
      allow-list (no wildcard origins) using the frontend port from T003, in
      `backend/RSSFeedReader.Api/Program.cs` (constitution Principle I)

**Checkpoint**: Foundation ready — user story implementation can now begin

## Phase 3: User Story 1 - Add a feed subscription by URL (Priority: P1) 🎯 MVP

**Goal**: A user pastes a feed URL into an input field, submits it, and it appears in the
subscription list without a page reload.

**Independent Test**: Enter a URL into the input field, submit it, and confirm it now appears in
the subscription list.

- [ ] T010 [US1] Implement the `POST /api/subscriptions` minimal API endpoint in
      `backend/RSSFeedReader.Api/Program.cs`: reject an empty or whitespace-only `url` with
      `400 Bad Request`, otherwise append it to `SubscriptionStore` and return `201 Created` with
      the created `Subscription` (contracts/subscriptions-api.md, FR-001, FR-004)
- [ ] T011 [US1] Implement the `GET /api/subscriptions` minimal API endpoint in
      `backend/RSSFeedReader.Api/Program.cs`, returning all subscriptions from
      `SubscriptionStore` in insertion order (contracts/subscriptions-api.md, FR-002)
- [ ] T012 [P] [US1] Create `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` with the
      `@page "/"` route, containing a URL text input and an "Add" button (FR-001)
- [ ] T013 [US1] In `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`, wire the "Add" button
      to call `POST /api/subscriptions` via `HttpClient` and refresh the displayed list on
      success, with no full page reload (FR-003)
- [ ] T014 [US1] In `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`, prevent submission
      when the input is empty or whitespace-only (edge case in [spec.md](./spec.md))
- [ ] T015 [P] [US1] Update `frontend/RSSFeedReader.UI/Layout/NavMenu.razor` to link to the
      subscriptions page with a label reflecting the MVP feature (e.g., "Subscriptions")

**Checkpoint**: User Story 1 should be fully functional and independently testable — a user can
add a subscription and see it appear in the list.

## Phase 4: User Story 2 - View the current subscription list (Priority: P2)

**Goal**: A user sees every subscription added during the current session displayed together in
the UI.

**Independent Test**: Load the app with subscriptions already present in memory and confirm they
are all rendered in the list.

- [ ] T016 [US2] In `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`, call
      `GET /api/subscriptions` during `OnInitializedAsync` to load the existing subscription list
      on page load (FR-002)
- [ ] T017 [US2] In `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`, render the loaded
      subscriptions as a simple list showing each entry's URL, in the order returned by the API
      (data-model.md, FR-002)

**Checkpoint**: User Story 2 should be fully functional and independently testable — reloading
the page shows every subscription added so far.

## Phase 5: Polish & Cross-Cutting Concerns

- [ ] T018 [P] Run the local development checklist in [quickstart.md](./quickstart.md) (backend
      runs without errors, frontend runs without errors, `ApiBaseUrl` correct, CORS allows the
      frontend origin, browser console free of connection errors)
- [ ] T019 [P] Execute the 4 validation scenarios in [quickstart.md](./quickstart.md) (add a
      subscription, add multiple subscriptions, reject blank input, confirm in-memory-only
      storage) and confirm the expected outcomes
- [ ] T020 Review `backend/RSSFeedReader.Api` and `frontend/RSSFeedReader.UI` for dead code,
      commented-out blocks, and unused template artifacts (constitution Principle III)

## Dependencies & Execution Order

- **Setup (Phase 1)**: T001 → T002/T003/T004 (parallel) — no dependencies
- **Foundational (Phase 2)**: Depends on Setup completion. T005 → T006. T007 → T008 → T009.
  T005/T007 can run in parallel with each other. **Blocks all user stories.**
- **User Story 1 (Phase 3)**: Depends on Foundational completion. T010 → T011 (same file,
  sequential). T012 can run in parallel with T010/T011. T013 depends on T010/T011/T012. T014
  depends on T013. T015 can run in parallel with any of the above.
- **User Story 2 (Phase 4)**: Depends on Foundational completion and on T011 (`GET` endpoint) and
  T012 (page) from User Story 1. T016 → T017 (same file, sequential).
- **Polish (Phase 5)**: Depends on all user story phases being complete.

### Parallel Example: Phase 1

```text
T002 [P] Configure backend/RSSFeedReader.Api/Properties/launchSettings.json
T003 [P] Configure frontend/RSSFeedReader.UI/Properties/launchSettings.json
T004 [P] Configure frontend/RSSFeedReader.UI/wwwroot/appsettings.json
```

### Parallel Example: Phase 2

```text
T005 [P] Remove template demo pages + NavMenu links
T007 [P] Create backend/RSSFeedReader.Api/Models/Subscription.cs
```

### Parallel Example: Phase 3 (User Story 1)

```text
T012 [P] [US1] Create frontend/RSSFeedReader.UI/Pages/Subscriptions.razor (input + button)
T015 [P] [US1] Update frontend/RSSFeedReader.UI/Layout/NavMenu.razor
```

## Implementation Strategy

1. Complete Phase 1 (Setup) and Phase 2 (Foundational) first — these block everything else.
2. Complete Phase 3 (User Story 1) to deliver the MVP: a user can add a subscription and see it
   in the list. **Stop here and validate against quickstart.md scenario 1–3 before continuing.**
3. Complete Phase 4 (User Story 2) to round out the "MVP working" definition from
   `StakeholderDocuments/ProjectGoals.md` (viewing the full list, including after reload).
4. Complete Phase 5 (Polish) to run the full local development checklist and validation scenarios.

**Suggested MVP scope**: Phases 1–3 (Setup, Foundational, User Story 1) — this alone satisfies
the "add a subscription and see it in the list" acceptance scenarios in [spec.md](./spec.md).
