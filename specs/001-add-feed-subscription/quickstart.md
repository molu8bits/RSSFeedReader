# Quickstart: Add Feed Subscription (MVP)

Validation guide for the "Add Feed Subscription" feature. Assumes the project structure and
contracts described in [plan.md](./plan.md), [data-model.md](./data-model.md), and
[contracts/subscriptions-api.md](./contracts/subscriptions-api.md).

## Prerequisites

- .NET 8 SDK installed.
- Template cleanup already done (`Home.razor`, `Counter.razor`, `Weather.razor` removed from
  `frontend/RSSFeedReader.UI/Pages/`, per constitution Principle III).

## Setup

```powershell
dotnet clean frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj
dotnet build backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj
dotnet build frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj
```

## Run

Start the backend and frontend in separate terminals:

```powershell
dotnet run --project backend/RSSFeedReader.Api
```

```powershell
dotnet run --project frontend/RSSFeedReader.UI
```

## Local development checklist (must pass before declaring this feature done)

- [ ] Backend runs without errors and listens on its configured port (see
      `backend/RSSFeedReader.Api/Properties/launchSettings.json`).
- [ ] Frontend runs without errors and loads in the browser.
- [ ] `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` `ApiBaseUrl` points at the backend port.
- [ ] Backend CORS policy allows the frontend origin.
- [ ] Browser DevTools console shows no connection errors when loading the page.
- [ ] Only one Razor page uses `@page "/"` (the subscriptions page); no ambiguous-route error
      appears in the console.

## Validation scenarios

1. **Add a subscription**
   - Open the frontend in a browser.
   - Paste a URL (e.g., `https://devblogs.microsoft.com/dotnet/feed/`) into the input field and
     submit.
   - Expected: the URL appears in the displayed subscription list immediately, no page reload.

2. **Add multiple subscriptions**
   - Repeat step 1 with at least 5 different URLs in the same session.
   - Expected: all submitted URLs are visible in the list simultaneously, in the order added
     (SC-003).

3. **Reject blank input**
   - Submit the form with an empty or whitespace-only input.
   - Expected: no new entry is added to the list (edge case in [spec.md](./spec.md)).

4. **Confirm in-memory-only storage**
   - Add a subscription, then restart the backend process.
   - Expected: the subscription list is empty after restart (FR-005).

## Out of scope for this validation

- Feed fetching, parsing, or item display (deferred to Extended-MVP).
- Removing subscriptions (deferred to Post-MVP).
