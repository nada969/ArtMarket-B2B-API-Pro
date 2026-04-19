# SETUP.md — Local Development Guide

> This guide walks you through getting ArtMarket running on your local machine from scratch. It covers standard setup, Docker, and Cursor AI integration.

---

## Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Clone & Explore](#2-clone--explore)
3. [Configure appsettings & Environment Variables](#3-configure-appsettings--environment-variables)
4. [Database Setup & Migrations](#4-database-setup--migrations)
5. [Run the Project Locally](#5-run-the-project-locally)
6. [Run with Docker](#6-run-with-docker)
7. [Using Cursor AI with This Project](#7-using-cursor-ai-with-this-project)
8. [Common Errors & Fixes](#8-common-errors--fixes)

---

## 1. Prerequisites

Ensure the following tools are installed before proceeding:

| Tool | Version | Purpose | Download |
|------|---------|---------|----------|
| **.NET SDK** | 10.0+ | Build and run the API | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0) |
| **PostgreSQL** | 14+ | Database | [postgresql.org](https://www.postgresql.org/download/) |
| **pgAdmin** | Any | PostgreSQL GUI client | [pgadmin.org](https://www.pgadmin.org/download/) |
| **EF Core CLI** | 10.0+ | Run migrations | `dotnet tool install -g dotnet-ef` |
| **Git** | Any | Clone the repo | [git-scm.com](https://git-scm.com/) |
| **Docker Desktop** | 4.x+ | *(Optional)* Containerized run | [docker.com](https://www.docker.com/products/docker-desktop/) |
| **Visual Studio 2022** | 17.8+ | *(Optional)* Full IDE | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| **VS Code + C# Dev Kit** | Any | *(Optional)* Lightweight IDE | [code.visualstudio.com](https://code.visualstudio.com/) |
| **Cursor AI** | Any | *(Optional)* AI-assisted IDE | [cursor.sh](https://cursor.sh/) |

### Verify your .NET installation

```bash
dotnet --version
# Expected output: 10.0.x
```

### Install EF Core CLI tools

```bash
dotnet tool install --global dotnet-ef
dotnet ef --version
# Expected output: Entity Framework Core .NET Command-line Tools 10.0.x
```

---

## 2. Clone & Explore

```bash
# Clone the repository
git clone https://github.com/nada969/ArtMarket-B2B-API-Pro.git

# Navigate into the project
cd ArtMarket-B2B-API-Pro

# Restore all NuGet packages
dotnet restore
```

---

## 3. Configure appsettings & Environment Variables

ArtMarket uses `appsettings.json` for configuration. Fill in your local values before running the project.

### Edit `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=DB_B2B;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-minimum-32-character-secret-key-here!!",
    "Issuer": "ArtMarketAPI",
    "Audience": "ArtMarketClients",
    "ExpiryMinutes": 60
  },
  "EmailSettings": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "SenderEmail": "noreply@yourdomain.com",
    "SenderName": "ArtMarket",
    "ApiKey": "your-sendgrid-api-key"
  },
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key",
    "Model": "gpt-4o"
  },
  "SubscriptionSettings": {
    "FreeListingLimit": 5
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

> ⚠️ **Never commit `appsettings.json` with real credentials.** Add it to `.gitignore` or use environment variables for secrets.

### Connection String Format

**PostgreSQL:**
```
Host=localhost;Port=5432;Database=DB_B2B;Username=postgres;Password=yourpassword
```

---

## 4. Database Setup & Migrations

### Apply existing migrations (first-time setup)

```bash
dotnet ef database update \
  --project ArtMarket.Infrastructure \
  --startup-project ArtMarket.API
```

This creates the database and applies all migrations, including the seed for the default admin user.

### Default Admin Credentials (Seeded)

| Field | Value |
|-------|-------|
| Email | `admin@artmarket.io` |
| Password | `Admin@123!` |

> ⚠️ Change the admin password immediately after first login in any non-local environment.

### Create a new migration (after changing domain entities)

```bash
dotnet ef migrations add YourMigrationName \
  --project ArtMarket.Infrastructure \
  --startup-project ArtMarket.API \
  --output-dir Migrations
```

### Roll back the last migration

```bash
# Revert database to the previous migration
dotnet ef database update PreviousMigrationName \
  --project ArtMarket.Infrastructure \
  --startup-project ArtMarket.API

# Remove the migration file
dotnet ef migrations remove \
  --project ArtMarket.Infrastructure \
  --startup-project ArtMarket.API
```

### List all applied migrations

```bash
dotnet ef migrations list \
  --project ArtMarket.Infrastructure \
  --startup-project ArtMarket.API
```

---

## 5. Run the Project Locally

### Run with Visual Studio (recommended)

Press **F5** or click the ▶ Run button. The API starts via IIS Express at:

```
https://localhost:44356
https://localhost:44356/swagger/index.html  ← Swagger UI
```

### Run with .NET CLI (terminal)

```bash
cd ArtMarket.API
dotnet run
```

The API starts via Kestrel at:

```
http://localhost:5112
http://localhost:5112/swagger/index.html  ← Swagger UI
```

### Run with Hot Reload

```bash
cd ArtMarket.API
dotnet watch run
```

### Run Tests

> Test projects are not set up yet — planned for v2.0. See [ROADMAP.md](ROADMAP.md) for details.

Once test projects are added, tests will run with:

```bash
dotnet test
```

---

## 6. Run with Docker

> Docker is not configured yet — planned for v2.0. See [ROADMAP.md](ROADMAP.md) for details.

Once configured, the setup will use:

```bash
# Start API + PostgreSQL in containers
docker-compose up --build

# Run in detached (background) mode
docker-compose up --build -d

# View logs
docker-compose logs -f artmarket-api

# Stop all services
docker-compose down

# Stop and remove volumes (wipes database)
docker-compose down -v
```

The `docker-compose.yml` will wire up the API container against a `postgres:16` container using the same connection string format as local development.

---

## 7. Using Cursor AI with This Project

[Cursor](https://cursor.sh/) is an AI-powered IDE built on VS Code. Here's how to get the best results using it with this codebase.

### Open the project in Cursor

```bash
cursor .
```

Or open Cursor and use **File → Open Folder** to select the project directory.

### Recommended `.cursorrules` Setup

Create a `.cursorrules` file in the project root:

```
You are an expert ASP.NET Core developer working on ArtMarket, an online marketplace for local artists.

Architecture:
- Clean layered architecture: Controllers → Services → Repositories → Database
- Never put business logic in controllers — delegate to services
- Use async/await throughout; every DB call is async
- Use DTOs at the API boundary; never expose domain entities directly
- Use the repository pattern; never call DbContext directly from services

Patterns & Conventions:
- Follow C# naming conventions: PascalCase for classes/methods, camelCase for locals
- Every service method should have a corresponding interface method
- Use FluentValidation for request validation, not Data Annotations
- Use AutoMapper for DTO mappings; mappings are defined in Application/Mappings/
- Return domain exceptions from services; controllers handle HTTP status codes
- Use dependency injection everywhere; never use `new` for services

When generating code, always include XML doc comments on public methods and classes.
```

### Useful Cursor AI Prompts

```
"Add a feature for artists to upload multiple images per artwork.
Follow the existing pattern in ArtworkService and ArtworkRepository."

"Why might the artist approval email not be sending?
Check EmailService and OrderController for the flow."

"I added a Tags property (List<string>) to the Artwork entity.
What EF Core migration code should I write?"
```

---

## 8. Common Errors & Fixes

### ❌ Unable to connect to PostgreSQL

**Error:**
```
Failed to connect to 127.0.0.1:5432
```
or
```
FATAL: password authentication failed for user "postgres"
```

**Fix:**
- Open **pgAdmin** and check if the server is running — it shows a green icon if active
- Or start it manually from Windows Services:
  ```bash
  net start postgresql-x64-16
  ```
- Verify your connection string in `appsettings.json` uses the correct format:
  ```
  Host=localhost;Port=5432;Database=DB_B2B;Username=postgres;Password=yourpassword
  ```
- Wrong password → open pgAdmin, right-click the `postgres` user → **Change Password**
- For Docker: verify the `postgres` container is healthy before starting the API:
  ```bash
  docker ps
  ```

---

### ❌ Connection string not found

**Error:**
```
InvalidOperationException: Connection string 'DefaultConnection' not found.
```

**Fix:**
- Right-click `appsettings.json` in Solution Explorer → **Properties** → set `Copy to Output Directory` to `Copy if newer`
- Verify the key name matches exactly — `"DefaultConnection"` in both `appsettings.json` and `Program.cs`
- Add this temporary debug line at the top of `Program.cs` to confirm the file is being read:
  ```csharp
  Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));
  ```
  If it prints `null` → the file is not being found at runtime

---

### ❌ Migrations pending — database not up to date

**Error:**
```
There are pending model changes. Run dotnet ef database update.
```

**Fix:**
```bash
dotnet ef database update \
  --project ArtMarket.Infrastructure \
  --startup-project ArtMarket.API
```

---

### ❌ No migration configuration type was found

**Error:**
```
Your startup project 'ArtMarket.API' doesn't reference Microsoft.EntityFrameworkCore.Design.
```

**Fix:**
```bash
dotnet add ArtMarket.Infrastructure package Microsoft.EntityFrameworkCore.Design
```

---

### ❌ JWT signature validation failed

**Error:**
```
Microsoft.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException
```

**Fix:**
- `JwtSettings__SecretKey` in `appsettings.json` must be **at least 32 characters**
- Make sure the same key is used for both token generation and validation
- Never hardcode or shorten the key

---

### ❌ CORS error from browser client

**Error:**
```
Access to XMLHttpRequest at 'https://localhost:44356' (IIS Express)
or 'http://localhost:5112' (Kestrel)
from origin 'http://localhost:3000' has been blocked by CORS policy.
```

**Fix:**
Add your frontend origin to the CORS policy in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
```

---

### ❌ OpenAI API call failed: 429 Too Many Requests

**Fix:**
- Check your usage limits at [platform.openai.com/usage](https://platform.openai.com/usage)
- For local development, mock the chatbot service to avoid consuming API credits:

```csharp
// Program.cs — swap real service for mock in Development
if (app.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IChatbotService, MockChatbotService>();
}
```

---

### ❌ dotnet watch not detecting file changes

**Fix:**
```bash
# Run from the folder containing the .csproj
cd ArtMarket.API
dotnet watch run

# If still not working, clear build artifacts first
dotnet clean
dotnet watch run
```