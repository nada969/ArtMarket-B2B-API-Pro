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
| **.NET SDK** | 8.0+ | Build and run the API | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **PostgreSQL Server** | Latest (18.x recommended) | Database | [postgresql.org](https://www.postgresql.org/download/) |
| **EF Core CLI** | 8.0+ | Run migrations | `dotnet tool install -g dotnet-ef` |
| **Git** | Any | Clone the repo | [git-scm.com](https://git-scm.com/) |
| **Docker Desktop** | 4.x+ | *(Optional)* Containerized run | [docker.com](https://www.docker.com/products/docker-desktop/) |
| **Visual Studio 2022** | 17.8+ | *(Optional)* Full IDE | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| **VS Code + C# Dev Kit** | Any | *(Optional)* Lightweight IDE | [code.visualstudio.com](https://code.visualstudio.com/) |
| **Cursor AI** | Any | *(Optional)* AI-assisted IDE | [cursor.sh](https://cursor.sh/) |

### Verify your .NET installation

```bash
dotnet --version
# Expected output: 8.0.x
```

### Install EF Core CLI tools

```bash
dotnet tool install --global dotnet-ef
dotnet ef --version
# Expected output: Entity Framework Core .NET Command-line Tools 8.0.x
```

---

## 2. Clone & Explore

```bash
# Clone the repository
git clone https://github.com/nada969/ArtMarket-B2B-API-Pro.git

# Navigate into the project
cd artmarket

# Restore all NuGet packages
dotnet restore
```

---

## 3. Configure appsettings & Environment Variables

ArtMarket uses `appsettings.json` for non-sensitive config and **environment variables** (or `appsettings.Development.json`) for secrets.

### Step 1: Copy the example config

```bash
cp appsettings.json 
```

### Step 2: Edit `appsettings.Development.json`

Open the file and fill in your local values:

```json
{
  "ConnectionStrings": { 
    "DefaultConnection": "Host=localhost;Port=PortNumber;Database=YourDBName;Username=YourUserName;Password=YpurPassword"
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
    "FreeListingLimit": 9
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

> ⚠️ **Never commit `appsettings.json` or any file containing secrets.** It is already listed in `.gitignore`.

### Connection String Variants

**PostgreSQL :**
```
Host=localhost;Database=artmarketdb;Username=postgres;Password=yourpassword;
```

### Using .env instead

If you prefer dotenv-style secrets, create a `.env` file in the repo root:

```bash
cp .env.example .env
```

Then populate it:

```
CONNECTIONSTRINGS__DEFAULTCONNECTION=Server=localhost\\SQLEXPRESS;Database=ArtMarketDb;Trusted_Connection=True;TrustServerCertificate=True;
JWTSETTINGS__SECRETKEY=your-minimum-32-character-secret-key!!
JWTSETTINGS__ISSUER=ArtMarketAPI
JWTSETTINGS__AUDIENCE=ArtMarketClients
JWTSETTINGS__EXPIRYMINUTES=60
EMAILSETTINGS__SMTPHOST=smtp.sendgrid.net
EMAILSETTINGS__SMTPPORT=587
EMAILSETTINGS__SENDEREMAIL=noreply@yourdomain.com
EMAILSETTINGS__APIKEY=your-sendgrid-api-key
OPENAI__APIKEY=sk-your-openai-api-key
OPENAI__MODEL=gpt-4o
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

### Run with .NET CLI

```bash
dotnet run --project ,ArtMarket.API
```


The API will start at:

```
- `https://localhost:44356` — when running via IIS Express (Visual Studio)
- `http://localhost:5112` — when running via `dotnet run` in terminal
```

Swagger UI (available in Development only):
```
https://localhost:44356/swagger/index.html
```

### Run with Hot Reload (recommended during development)

```bash
dotnet watch run 
```

### Run Tests

> Test files are not set up yet — planned for v2.0.
> See [ROADMAP.md](ROADMAP.md) for details.

Once test projects are added, tests will run with:

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run only unit tests
dotnet test tests/ArtMarket.UnitTests

# Run only integration tests
dotnet test tests/ArtMarket.IntegrationTests
```

---

## 6. Run with Docker

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running

### Start all services with Docker Compose

```bash
# Start API + SQL Server in containers
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

### `docker-compose.yml` Overview

```yaml
version: '3.9'

services:
  artmarket-api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "7001:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=ArtMarketDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;
      - JwtSettings__SecretKey=your-minimum-32-character-secret-key!!
      - JwtSettings__Issuer=ArtMarketAPI
      - JwtSettings__Audience=ArtMarketClients
      - EmailSettings__SmtpHost=smtp.sendgrid.net
      - EmailSettings__ApiKey=your-sendgrid-key
      - OpenAI__ApiKey=sk-your-openai-key
    depends_on:
      - sqlserver

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=YourStrongPassword123!
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql

volumes:
  sqlserver-data:
```

### Access the API in Docker

Once running, the API is available at:
```
http://localhost:7001
http://localhost:7001/swagger
```

---

## 7. Using Cursor AI with This Project

[Cursor](https://cursor.sh/) is an AI-powered IDE built on VS Code. Here's how to get the best results using it with this codebase.

### Open the project in Cursor

```bash
cursor .
```

Or open Cursor and use **File → Open Folder** to select the `artmarket/` directory.

### Recommended `.cursorrules` Setup

Create a `.cursorrules` file in the project root to give Cursor context about the project:

```
You are an expert ASP.NET Core 8 developer working on ArtMarket, an online marketplace for local artists.

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

Testing:
- Unit tests use xUnit + Moq
- Mock all dependencies at the service layer
- Integration tests use WebApplicationFactory with an in-memory database

When generating code, always include XML doc comments on public methods and classes.
```

### Useful Cursor AI Prompts

```
# Generate a new feature end-to-end
"Add a feature for artists to upload multiple images per artwork. 
Follow the existing pattern in ArtworkService and ArtworkRepository."

# Debug an issue
"Why might the artist approval email not be sending? 
Check EmailService and OrderController for the flow."

# Write tests
"Write xUnit unit tests for ArtworkService.CreateAsync() covering: 
free-tier limit enforcement, successful creation, and unapproved artist rejection."

# Generate a migration
"I added a Tags property (List<string>) to the Artwork entity. 
What EF Core migration code should I write?"
```

---

## 8. Common Errors & Fixes

### ❌ `Unable to connect to SQL Server`

**Error:**
```
A network-related or instance-specific error occurred while establishing a connection to SQL Server.
```

**Fix:**
- Verify SQL Server is running: check Windows Services or `docker ps`
- Check the connection string in `appsettings.Development.json`
- For SQL Express: use `Server=localhost\\SQLEXPRESS` (double backslash)
- For Docker: ensure `sqlserver` container is healthy before the API starts

---

### ❌ `Migrations pending — database may not be up to date`

**Error:**
```
There are pending model changes. Run dotnet ef database update.
```

**Fix:**
```bash
dotnet ef database update \
  --project src/ArtMarket.Infrastructure \
  --startup-project src/ArtMarket.API
```

---

### ❌ `IDX10503: Signature validation failed`

**Error:**
```
Microsoft.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException
```

**Fix:**
- Your `JwtSettings__SecretKey` in `appsettings.Development.json` must be **at least 32 characters**
- Ensure the same key is used for both token generation and validation
- Never use the example key in production

---

### ❌ `No migration configuration type was found`

**Error:**
```
Your startup project 'ArtMarket.API' doesn't reference Microsoft.EntityFrameworkCore.Design.
```

**Fix:**
Ensure `ArtMarket.Infrastructure` has the EF Core Design package:

```bash
dotnet add src/ArtMarket.Infrastructure package Microsoft.EntityFrameworkCore.Design
```

---

### ❌ `CORS error from browser client`

**Error:**
```
Access to XMLHttpRequest at 'https://localhost:7001' from origin 'http://localhost:3000' has been blocked by CORS policy.
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

### ❌ `OpenAI API call failed: 429 Too Many Requests`

**Fix:**
- Check your OpenAI API usage limits at [platform.openai.com/usage](https://platform.openai.com/usage)
- For local development, you can mock the chatbot service:

```csharp
// In Program.cs, add a mock for development
if (app.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IChatbotService, MockChatbotService>();
}
```

---

### ❌ `dotnet watch` not detecting file changes

**Fix:**
```bash
# Ensure you're running from the correct directory
cd artmarket
dotnet watch run --project src/ArtMarket.API

# If still not working, clear build artifacts
dotnet clean
dotnet watch run --project src/ArtMarket.API
```