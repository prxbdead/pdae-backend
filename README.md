# PDAE — .NET 10 backend

ASP.NET Core Web API targeting **.NET 10** (`net10.0`), scaffolded with the `webapi` template. OpenAPI is enabled in Development.

## Requirements

- .NET SDK 10 (`dotnet --version` → `10.0.403` or newer)

> If `dotnet --version` reports 9.x, make sure the .NET 10 Homebrew path comes first:
> `export PATH="/opt/homebrew/opt/dotnet@10/bin:$PATH"`

## Run

```sh
dotnet run --project pdae-backend/Pdae.Api.csproj
```

The HTTP profile listens on <http://localhost:5215>.

## Endpoints

| Method | Route              | Description                          |
| ------ | ------------------ | ------------------------------------ |
| GET    | `/health`          | Liveness probe used by the mobile app |
| GET    | `/weatherforecast` | Sample data from the template         |
| GET    | `/openapi/v1.json` | OpenAPI document (Development only)   |

## Project layout

```
pdae-backend/
├── Pdae.slnx                 # Solution (new XML solution format)
├── Pdae.Api.csproj           # net10.0 web project
├── Program.cs                # Minimal API endpoints + CORS
├── Pdae.Api.http             # Ready-to-run HTTP requests
└── Properties/launchSettings.json
```

CORS is open (`AllowAnyOrigin`) for local development so browser-based debugging works. Tighten this before deploying.
