# ShortUrl

A simple example project for a URL shortening (redirect) service with management APIs.

Key Features

- Web Service (`ShortUrl.WebApp`): exposes two types of endpoints:
 - Public redirect: `GET /{shortUrl}` — resolves the `shortCode` and redirects to the `LongUrl`.
 - Management API under `/api/v1/` protected by API key: `ping`, `create`, `edit`.
- Persistence (`ShortUrl.DbPersistence`): Entity Framework Core with entities for ApiKeys, ShortUrls, and ClickEvents.
- Support Application (`ShortUrl.DevSupport`): console app used to apply EF migrations and run development seed data.
- Distributed Host (`ShortUrl.AppHost`): defines local services (Postgres, Redis) and projects to launch in the development environment.

Development Seed

The `ShortUrl.DevSupport` project adds two example API keys when run with the `seed` argument:
- `api-key-local` (basic permissions)
- `api-key-local-custom-url` (permission to set custom short codes)

Useful Commands

- Create a migration (run from the `src` folder):

 `dotnet ef migrations add <MigrationName> --project ShortUrl.DbPersistence --startup-project ShortUrl.DevSupport`

- Apply migrations and seed with the support app (from `src` folder):

 `dotnet run --project ShortUrl.DevSupport -- migrate seed`

- Start the Web service (from `src/ShortUrl.WebApp` folder or from `src` with `--project`):

 `dotnet run --project ShortUrl.WebApp`

- Start the development host (if available, launches Postgres/Redis and defined projects):

 `dotnet run --project ShortUrl.AppHost`

Key Points

- The redirect route reads request information (IP, User-Agent, Referer) and writes a click event to a channel for asynchronous processing.
- Management APIs use validation (`FluentValidation`) and require an API key present in the `ApiKeys` table for protected operations.

Contacts

- Repository: local in this folder. For contributions, open pull requests on the main remote.

