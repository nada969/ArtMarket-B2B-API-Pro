# CONTRIBUTING.md — Contribution Guide

> Thank you for your interest in contributing to ArtMarket! This document explains everything you need to know to get your changes merged cleanly.

---

## Table of Contents

1. [Getting Started](#1-getting-started)
2. [Branch Naming Conventions](#2-branch-naming-conventions)
3. [Commit Message Format](#3-commit-message-format)
4. [Pull Request Checklist](#4-pull-request-checklist)
5. [Code Style Guidelines](#5-code-style-guidelines)
6. [Reporting Issues](#6-reporting-issues)

---

## 1. Getting Started

### Fork & Clone

```bash
# 1. Fork the repository on GitHub (click "Fork" on the repo page)

# 2. Clone your fork
git clone https://github.com/YOUR-USERNAME/artmarket.git
cd artmarket

# 3. Add the upstream remote so you can pull future changes
git remote add upstream https://github.com/your-org/artmarket.git

# 4. Verify remotes
git remote -v
# origin    https://github.com/YOUR-USERNAME/artmarket.git (fetch)
# upstream  https://github.com/your-org/artmarket.git (fetch)
```

### Keep Your Fork in Sync

Before starting any new feature, sync your fork with the upstream `main`:

```bash
git checkout main
git fetch upstream
git merge upstream/main
git push origin main
```

### Set Up Locally

Follow [SETUP.md](SETUP.md) to get the project running locally before making changes.

---

## 2. Branch Naming Conventions

All branches must be created from `main` and follow this naming convention:

```
<type>/<short-description>
```

| Type | Purpose | Example |
|------|---------|---------|
| `feat` | New feature | `feat/artist-profile-image-upload` |
| `fix` | Bug fix | `fix/order-email-not-sending` |
| `refactor` | Code restructure, no behavior change | `refactor/artwork-service-cleanup` |
| `test` | Adding or fixing tests | `test/artwork-service-unit-tests` |
| `docs` | Documentation only | `docs/update-api-reference` |
| `chore` | Build, CI, config changes | `chore/upgrade-dotnet-8` |
| `hotfix` | Critical production fix | `hotfix/jwt-expiry-bug` |

### Examples

```bash
git checkout -b feat/chatbot-history-support
git checkout -b fix/admin-approval-404-error
git checkout -b docs/add-postman-collection
```

---

## 3. Commit Message Format

ArtMarket follows the [Conventional Commits](https://www.conventionalcommits.org/) specification.

### Format

```
<type>(<scope>): <short description>

[optional body]

[optional footer(s)]
```

### Rules

- **Type** — one of: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `style`, `perf`
- **Scope** — the affected module or feature: `auth`, `artwork`, `orders`, `chatbot`, `admin`, `email`, `subscriptions`
- **Short description** — imperative mood, lowercase, no period, max 72 characters
- **Body** — optional; explain *why*, not *what* (the diff shows what)
- **Footer** — optional; reference issues with `Closes #123` or `Fixes #456`

### Examples

```
feat(artwork): add support for multiple image uploads per listing

Previously only a single imageUrl was supported. This adds an ArtworkImages
table and updates the CreateArtwork endpoint to accept an array of URLs.

Closes #42
```

```
fix(email): prevent null reference when artist email is missing

The order notification was throwing NullReferenceException when the
artist's email field was not populated. Added a null guard in EmailService.

Fixes #88
```

```
refactor(chatbot): extract OpenAI client to dedicated wrapper class

Moved the raw HttpClient calls from ChatbotService into a dedicated
OpenAIClient class for better testability and separation of concerns.
```

```
test(orders): add unit tests for OrderService status transition rules
```

---

## 4. Pull Request Checklist

Before opening a PR, verify every item on this list:

### Code Quality

- [ ] All new public classes and methods have XML doc comments (`/// <summary>`)
- [ ] No TODO comments left in new code (open a GitHub issue instead)
- [ ] No commented-out code in the PR
- [ ] No hardcoded strings, secrets, or magic numbers — use constants or config
- [ ] No business logic in controllers (delegate to services)
- [ ] No raw `DbContext` calls outside of repositories

### Tests

- [ ] All new service methods have corresponding unit tests in `ArtMarket.UnitTests`
- [ ] All new endpoints have at least one integration test in `ArtMarket.IntegrationTests`
- [ ] All existing tests pass: `dotnet test`
- [ ] New tests are isolated — they do not depend on test execution order

### Build & Formatting

- [ ] Project builds with no warnings: `dotnet build --no-restore`
- [ ] No new NuGet packages added without discussion in the PR description
- [ ] `appsettings.Development.json` or `.env` files are NOT committed
- [ ] `appsettings.Example.json` is updated if new config keys were added

### PR Description

- [ ] Title follows commit format: `feat(scope): description`
- [ ] Description explains *what* changed and *why*
- [ ] Relevant GitHub issues are linked (e.g., `Closes #42`)
- [ ] Breaking changes (if any) are clearly documented

---

## 5. Code Style Guidelines

ArtMarket follows standard C# and ASP.NET Core conventions. Refer to the [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

### Naming

```csharp
// ✅ Classes: PascalCase
public class ArtworkService { }

// ✅ Interfaces: IPascalCase
public interface IArtworkService { }

// ✅ Methods: PascalCase, async methods end in Async
public async Task<ArtworkDto> CreateAsync(CreateArtworkDto dto) { }

// ✅ Local variables and parameters: camelCase
var artworkId = Guid.NewGuid();

// ✅ Private fields: _camelCase
private readonly IArtworkRepository _artworkRepository;

// ✅ Constants: PascalCase
public const int MaxImageSizeMb = 5;
```

### Async/Await

```csharp
// ✅ Always use async/await for I/O-bound operations
public async Task<List<ArtworkDto>> GetAllAsync()
{
    var artworks = await _artworkRepository.GetAllAsync();
    return _mapper.Map<List<ArtworkDto>>(artworks);
}

// ❌ Never block async code with .Result or .Wait()
var artworks = _artworkRepository.GetAllAsync().Result; // BAD
```

### Dependency Injection

```csharp
// ✅ Constructor injection — always preferred
public class ArtworkService
{
    private readonly IArtworkRepository _artworkRepository;
    private readonly ISubscriptionService _subscriptionService;

    public ArtworkService(
        IArtworkRepository artworkRepository,
        ISubscriptionService subscriptionService)
    {
        _artworkRepository = artworkRepository;
        _subscriptionService = subscriptionService;
    }
}
```

### Controllers

```csharp
// ✅ Thin controllers — only handle HTTP concerns
[HttpPost]
[Authorize(Roles = "Artist")]
public async Task<IActionResult> Create([FromBody] CreateArtworkDto dto)
{
    // Validate via FluentValidation filter (automatic)
    var result = await _artworkService.CreateAsync(User.GetArtistId(), dto);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}

// ❌ No business logic in controllers
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateArtworkDto dto)
{
    // BAD: querying DB directly, enforcing rules here
    var count = await _context.Artworks.CountAsync(a => a.ArtistId == artistId);
    if (count >= 5) return Forbid();
    // ...
}
```

### Error Handling

Domain exceptions are thrown from services and caught by the global exception handler middleware:

```csharp
// ✅ Define domain exceptions in ArtMarket.Domain/Exceptions/
public class ListingLimitExceededException : Exception
{
    public ListingLimitExceededException()
        : base("Free tier limit reached. Upgrade to Premium for unlimited listings.") { }
}

// ✅ Throw from service
if (currentCount >= limit)
    throw new ListingLimitExceededException();

// ✅ Global handler maps to HTTP status codes
// Middleware/GlobalExceptionHandlerMiddleware.cs handles the mapping
```

---

## 6. Reporting Issues

### Bug Reports

Please include:
1. Steps to reproduce the issue
2. Expected behavior
3. Actual behavior
4. Environment details (.NET version, OS, SQL Server version)
5. Relevant logs or stack traces

Use the **Bug Report** issue template on GitHub.

### Feature Requests

Before proposing a feature:
- Check the [ROADMAP.md](ROADMAP.md) — it may already be planned
- Check existing issues and discussions

Use the **Feature Request** issue template on GitHub and describe:
1. The problem you're trying to solve
2. Your proposed solution
3. Any alternatives you've considered

---

## Thank You

Every contribution — whether it's a typo fix, a failing test, a new feature, or a documentation improvement — makes ArtMarket better for artists and buyers everywhere. We review PRs as promptly as possible and will always leave constructive feedback.