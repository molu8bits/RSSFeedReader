# Phase 1 Data Model: Add Feed Subscription (MVP)

## Entity: Subscription

Represents a single feed the user wants to track. Matches the Key Entities section of
[spec.md](./spec.md).

| Field | Type   | Required | Notes |
|-------|--------|----------|-------|
| `Url` | string | Yes      | Raw text the user submitted. Not validated as a well-formed or reachable URL for this feature (FR-004). |

### Validation rules

- `Url` MUST NOT be empty or whitespace-only before being added to the store (edge case in
  [spec.md](./spec.md)). Any other content is accepted as-is — no format or reachability checks
  (FR-004).

### Relationships

- None. `Subscription` is a standalone record for this feature; no relationship to feed items
  exists yet (feed fetching/items are out of scope, FR-006).

### State transitions

- **Created**: added via `POST /api/subscriptions` and appended to the in-memory list, preserving
  insertion order (FR-002).
- No update, delete, or status transitions exist for this feature — removal is explicitly
  deferred (see Assumptions in [spec.md](./spec.md)).

### Storage shape

Held in-memory for the lifetime of the backend process as an ordered list of `Subscription`
records (see [research.md](./research.md) — singleton store decision). Not persisted; lost on
restart (FR-005).
