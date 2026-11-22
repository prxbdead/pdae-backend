
# PDAE — .NET 10 backend

ASP.NET Core Web API targeting **.NET 10** (`net10.0`).
OpenAPI is enabled in Development.

## Requirements

- .NET SDK 10 (`dotnet --version` → `10.0.403` or newer)
- Docker (for PostgreSQL)

> If `dotnet --version` reports 9.x, make sure the .NET 10 Homebrew path comes first:
> `export PATH="/opt/homebrew/opt/dotnet@10/bin:$PATH"`

## Run

```sh
dotnet run --project pdae-backend/Pdae.Api.csproj
```

The HTTP profile listens on http://localhost:5215.

## Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/health` | Liveness probe used by the mobile app |
| GET | `/openapi/v1.json` | OpenAPI document (Development only) |

## Database

PostgreSQL 18 runs through Docker Compose on host port **5433** (leaving a local Postgres on 5432 untouched):

```sh
docker compose up -d
```

The development connection string lives in `appsettings.Development.json`:

```
Host=localhost;Port=5433;Database=pdae;Username=pdae;Password=pdae_dev_password
```

Override it in any environment with `ConnectionStrings__DefaultConnection`.
The API fails fast at startup when no connection string is configured.

### Migrations

EF Core tooling is pinned in `dotnet-tools.json`:

```sh
dotnet tool restore
dotnet ef migrations add <Name> --output-dir Data/Migrations
dotnet ef database update
```

## Project layout

```

pdae-backend/
├── Pdae.slnx                 # Solution (new XML solution format)
├── Pdae.Api.csproj           # net10.0 web project
├── Program.cs                # Health endpoint + CORS + DbContext
├── Controllers/              # API controllers
│   └── .gitkeep
├── Models/                   # Domain entities and data models
│   └── .gitkeep
├── DTOs/                     # Data Transfer Objects (requests/responses)
│   └── .gitkeep
├── Services/                 # Business logic and application services
│   └── .gitkeep
├── Data/
│   ├── AppDbContext.cs       # EF Core context
│   └── Migrations/           # Generated migrations
├── docker-compose.yml        # PostgreSQL 18 for local development
├── dotnet-tools.json         # Pinned dotnet-ef tool
├── Pdae.Api.http             # HTTP endpoint testing
└── Properties/
    └── launchSettings.json
```

## CORS

CORS is configured for local development using the
`Cors:AllowedOrigins` setting in `appsettings.Development.json`.

Only explicitly configured origins are allowed.

The development CORS policy is applied only when the
application runs in the Development environment.

Production CORS must be configured separately if needed.


## Authentication

The backend uses ASP.NET Core Identity with Entity Framework
Core and PostgreSQL.

### Identity components

- ApplicationUser extends IdentityUser.
- AppDbContext inherits from IdentityDbContext<ApplicationUser>.
- Identity services support user management, password hashing,
  role management, and cookie authentication.
- Identity database tables are managed through EF Core migrations.

### Database setup

Start PostgreSQL:

```sh
docker compose up -d
```

Apply migrations:

```sh
dotnet tool restore
dotnet ef database update
```

### Authentication endpoints

Registration, login, logout, and profile endpoints are
implemented in subsequent issues.

Cookie authentication requires HTTPS. CSRF protection and
end-to-end session handling will be completed with the
authentication endpoints.
