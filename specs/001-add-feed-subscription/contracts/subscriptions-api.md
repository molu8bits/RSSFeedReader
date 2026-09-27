# API Contract: Subscriptions

Backend: `RSSFeedReader.Api` — base path `/api`. Consumed by `RSSFeedReader.UI` via
`HttpClient` (`ApiBaseUrl` configuration, see [research.md](../research.md)).

## `GET /api/subscriptions`

Returns every subscription added during the current session, in the order added.

**Response** `200 OK`

```json
[
  { "url": "https://devblogs.microsoft.com/dotnet/feed/" },
  { "url": "https://example.com/feed.xml" }
]
```

- Empty session → `200 OK` with `[]` (never a 404).

## `POST /api/subscriptions`

Adds a new subscription URL to the in-memory list.

**Request**

```json
{ "url": "https://devblogs.microsoft.com/dotnet/feed/" }
```

**Response** `201 Created`

```json
{ "url": "https://devblogs.microsoft.com/dotnet/feed/" }
```

- `400 Bad Request` if `url` is missing, empty, or whitespace-only (edge case in
  [spec.md](../spec.md)).
- No format/reachability validation is performed beyond the empty/whitespace check (FR-004).
- Duplicate URLs are accepted and stored again — no de-duplication in this feature.

## Notes

- No authentication/authorization headers are required (single-user local MVP).
- CORS: the API MUST only accept requests from the configured frontend origin(s) — see
  [research.md](../research.md) CORS decision.
