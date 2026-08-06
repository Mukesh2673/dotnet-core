# MyFirstApi

A minimal ASP.NET Core Web API (.NET 9) using Entity Framework Core. The default
configuration talks to **PostgreSQL**, and instructions for switching to
**SQL Server** are included below.

---

## Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 9.0 or later | `dotnet --version` |
| PostgreSQL | 13+ | Default database provider |
| SQL Server | 2019+ / LocalDB / Docker | Only if you switch providers |
| `dotnet-ef` CLI | 9.x | Installed in the next step |

Install the EF Core CLI tool once per machine:

```bash
dotnet tool install --global dotnet-ef
# already installed? keep it current:
dotnet tool update --global dotnet-ef
```

Make sure the tools folder is on your `PATH` (add to `~/.bashrc` if needed):

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

---

## 1. Restore and build

```bash
cd MyFirstApi
dotnet restore
dotnet build
```

---

## 2. Configure the database

### Option A — PostgreSQL (default)

**2.1 Start PostgreSQL**

Local install:

```bash
sudo systemctl start postgresql
sudo systemctl status postgresql
```

Or with Docker:

```bash
docker run --name myfirstapi-postgres \
  -e POSTGRES_PASSWORD=YourPassword \
  -e POSTGRES_DB=MyFirstDb \
  -p 5432:5432 \
  -d postgres:16
```

**2.2 Create the database** (skip if Docker already created it)

```bash
sudo -u postgres psql -c "CREATE DATABASE \"MyFirstDb\";"
```

**2.3 Set the connection string**

Edit `appsettings.json` and replace `YourPassword` with your real
`postgres` password:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MyFirstDb;Username=postgres;Password=YourPassword;"
  }
}
```

> Prefer not to commit secrets? Use user secrets instead (see
> [Keeping the password out of source control](#keeping-the-password-out-of-source-control)).

**2.4 Verify `Program.cs` uses the Npgsql provider**

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

**2.5 Apply migrations**

```bash
dotnet ef database update
```

---

### Option B — SQL Server

The `Microsoft.EntityFrameworkCore.SqlServer` package is already referenced in
`MyFirstApi.csproj`, so no package install is needed.

**2.1 Start SQL Server**

Docker (works on Linux):

```bash
docker run --name myfirstapi-mssql \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Your_Strong_Password123" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

On Windows you can use LocalDB instead — it ships with Visual Studio.

**2.2 Set the connection string**

Replace the `DefaultConnection` value in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MyFirstDb;User Id=sa;Password=Your_Strong_Password123;TrustServerCertificate=True;"
  }
}
```

LocalDB variant (Windows):

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyFirstDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

**2.3 Switch the provider in `Program.cs`**

Change `UseNpgsql` to `UseSqlServer`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

**2.4 Regenerate the migrations**

EF Core migrations are **provider-specific** — the existing files under
`Migrations/` were generated for PostgreSQL and will not run against SQL Server.
Delete them and create a fresh migration:

```bash
rm -rf Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> On Windows PowerShell use `Remove-Item -Recurse -Force Migrations` instead of `rm -rf`.

---

## 3. Run the application

```bash
dotnet run
```

Or with hot reload while developing:

```bash
dotnet watch run
```

The launch profiles in `Properties/launchSettings.json` define the URLs:

| Profile | URLs |
|---------|------|
| `http` | http://localhost:5157 |
| `https` | https://localhost:7140 and http://localhost:5157 |

Pick one explicitly with:

```bash
dotnet run --launch-profile https
```

First time using HTTPS locally, trust the development certificate:

```bash
dotnet dev-certs https --trust
```

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/` | Welcome message |
| GET | `/weatherforecast` | Sample forecast data |
| GET | `/openapi/v1.json` | OpenAPI document (Development only) |

Quick check:

```bash
curl http://localhost:5157/
curl http://localhost:5157/weatherforecast
```

You can also use `MyFirstApi.http` directly from VS Code (REST Client) or
Visual Studio.

---

## Working with migrations

```bash
# create a new migration after changing a model
dotnet ef migrations add AddSomething

# apply pending migrations
dotnet ef database update

# roll back the last migration (before it was applied)
dotnet ef migrations remove

# see the SQL without running it
dotnet ef migrations script
```

---

## Keeping the password out of source control

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=MyFirstDb;Username=postgres;Password=YourPassword;"
```

User secrets override `appsettings.json` in the Development environment, so you
can leave a placeholder password in the committed file.

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `28P01: password authentication failed` | Wrong PostgreSQL password in the connection string. |
| `Npgsql.NpgsqlException: Connection refused` | PostgreSQL is not running, or the port is not 5432. |
| `A network-related or instance-specific error` (SQL Server) | Container not running, or add `TrustServerCertificate=True`. |
| `dotnet ef` not found | Install the tool and add `~/.dotnet/tools` to `PATH`. |
| `relation "Products" does not exist` | Migrations were never applied — run `dotnet ef database update`. |
| Migration errors after switching providers | Delete `Migrations/` and regenerate (see Option B step 2.4). |

---

## Project structure

```
MyFirstApi/
├── Controllers/          # API controllers
├── Data/
│   └── AppDbContext.cs   # EF Core DbContext (Products)
├── Migrations/           # EF Core migrations (provider-specific)
├── Models/
│   └── Prduct.cs         # Product entity
├── Properties/
│   └── launchSettings.json
├── appsettings.json      # Connection string lives here
└── Program.cs            # Startup, DI, endpoints
```
