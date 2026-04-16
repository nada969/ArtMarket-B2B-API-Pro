# ARCHITECTURE.md — ArtMarket Technical Design

> This document describes the internal architecture of the ArtMarket API. It is intended for contributors, maintainers, and engineers who want to understand how the system is structured before making changes.

---

## Table of Contents

1. [System Design Overview](#1-system-design-overview)
2. [Layer Breakdown](#2-layer-breakdown)
3. [Folder Structure](#3-folder-structure)
4. [Authentication & Authorization Flow](#4-authentication--authorization-flow)
5. [Subscription & Tier System](#5-subscription--tier-system)
6. [Email Notification System](#6-email-notification-system)
7. [Chatbot Integration](#7-chatbot-integration)
8. [Database Entity Relationships](#8-database-entity-relationships)

---

## 1. System Design Overview

ArtMarket is a RESTful API built on ASP.NET Core 8. It follows a **clean layered architecture** — no domain logic leaks into controllers, and no database code leaks into services. All external integrations (email, AI, payment) are abstracted behind interfaces to support testability and replacement.

### High-Level System Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                        External Clients                       │
│         (Browser / Mobile App / Postman / Frontend SPA)       │
└──────────────────────┬───────────────────────────────────────┘
                       │ HTTPS (JWT Bearer Token)
                       ▼
┌──────────────────────────────────────────────────────────────┐
│                    ASP.NET Core 8 API                         │
│  ┌──────────────┐  ┌───────────────┐  ┌──────────────────┐  │
│  │  Auth Layer  │  │  Middleware   │  │  Swagger/OpenAPI │  │
│  │  (JWT/Roles) │  │  (Logging,   │  │  (Dev only)      │  │
│  └──────────────┘  │   Error,CORS) │  └──────────────────┘  │
│                    └───────────────┘                          │
│  ┌────────────────────────────────────────────────────────┐  │
│  │                    Controllers                          │  │
│  │  AuthController  ArtistController  OrderController     │  │
│  │  AdminController BuyerController   ChatbotController   │  │
│  └────────────────────────────┬───────────────────────────┘  │
│                               │                               │
│  ┌────────────────────────────▼───────────────────────────┐  │
│  │                    Service Layer                        │  │
│  │  IAuthService     IArtistService   IOrderService        │  │
│  │  IAdminService    IArtworkService  IChatbotService      │  │
│  │  IEmailService    ISubscriptionService                  │  │
│  └────────────────────────────┬───────────────────────────┘  │
│                               │                               │
│  ┌────────────────────────────▼───────────────────────────┐  │
│  │                  Repository Layer                       │  │
│  │  IUserRepository       IArtworkRepository              │  │
│  │  IArtistRepository     IOrderRepository                │  │
│  └────────────────────────────┬───────────────────────────┘  │
│                               │ EF Core                       │
│  ┌────────────────────────────▼───────────────────────────┐  │
│  │              ArtMarketDbContext (EF Core)               │  │
│  └────────────────────────────┬───────────────────────────┘  │
└──────────────────────────────-┼──────────────────────────────┘
                                │
               ┌────────────────▼────────────────┐
               │         SQL Server / PostgreSQL  │
               └─────────────────────────────────┘

External Services:
  ┌─────────────┐    ┌──────────────┐    ┌──────────────┐
  │  SendGrid   │    │  OpenAI API  │    │  (Future)    │
  │  (Email)    │    │  (Chatbot)   │    │  Stripe      │
  └─────────────┘    └──────────────┘    └──────────────┘
```

---

## 2. Layer Breakdown

### Controllers → Services → Repositories → Database

Each layer has a single responsibility. Communication between layers flows strictly downward — no layer reaches past the one below it.

#### Controllers (`ArtMarket.API`)
- Receive HTTP requests and return HTTP responses
- Validate model binding and return `400 Bad Request` for invalid input
- Call one or more service methods to fulfill the request
- Map service results to API response DTOs
- Apply `[Authorize]` and `[Authorize(Roles = "...")]` attributes

```csharp
// Example: ArtworkController
[HttpPost]
[Authorize(Roles = "Artist")]
public async Task<IActionResult> CreateArtwork([FromBody] CreateArtworkDto dto)
{
    var artistId = User.GetArtistId(); // Extension method on ClaimsPrincipal
    var result = await _artworkService.CreateAsync(artistId, dto);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}
```

#### Services (`ArtMarket.Application`)
- Contain all business logic
- Enforce rules such as free-tier limits, approval checks, notification triggers
- Coordinate between multiple repositories when needed
- Dispatch emails asynchronously via `IEmailService`
- Return domain results — never `HttpStatusCode` or `IActionResult`

```csharp
// Example: ArtworkService
public async Task<ArtworkDto> CreateAsync(Guid artistId, CreateArtworkDto dto)
{
    var artist = await _artistRepo.GetByIdAsync(artistId);
    await _subscriptionService.EnforceListingLimitAsync(artist);
    var artwork = _mapper.Map<Artwork>(dto);
    artwork.ArtistId = artistId;
    await _artworkRepo.AddAsync(artwork);
    return _mapper.Map<ArtworkDto>(artwork);
}
```

#### Repositories (`ArtMarket.Infrastructure`)
- Wrap all EF Core database interactions
- Expose clean async CRUD methods
- Never contain business logic — only query construction
- Use the `DbContext` directly; never reference services

```csharp
// Example: ArtworkRepository
public async Task<IEnumerable<Artwork>> GetByArtistIdAsync(Guid artistId) =>
    await _context.Artworks
        .Where(a => a.ArtistId == artistId && !a.IsDeleted)
        .OrderByDescending(a => a.CreatedAt)
        .ToListAsync();
```

#### Database (`SQL Server / PostgreSQL via EF Core`)
- Schema managed entirely through EF Core migrations
- Seed data for initial admin user included in a migration
- All migrations tracked under `ArtMarket.Infrastructure/Migrations/`

---

## 3. Folder Structure

```
artmarket/
│
├── src/
│   ├── ArtMarket.API/                  # Presentation layer (Web API)
│   │   ├── Controllers/                # HTTP endpoint handlers
│   │   ├── Middleware/                 # Global error handler, logging
│   │   ├── Extensions/                 # Service registration, ClaimsPrincipal helpers
│   │   ├── Filters/                    # Action filters (e.g., validation)
│   │   ├── Program.cs                  # App bootstrap, DI container setup
│   │   └── appsettings.json            # Configuration (non-secret)
│   │
│   ├── ArtMarket.Application/          # Business logic layer
│   │   ├── Services/                   # Concrete service implementations
│   │   ├── Interfaces/                 # IService contracts
│   │   ├── DTOs/                       # Request/Response data transfer objects
│   │   ├── Mappings/                   # AutoMapper profiles
│   │   └── Validators/                 # FluentValidation validators
│   │
│   ├── ArtMarket.Domain/               # Core domain models (no dependencies)
│   │   ├── Entities/                   # Artist, Artwork, Order, User, etc.
│   │   ├── Enums/                      # ArtistStatus, SubscriptionTier, OrderStatus
│   │   └── Exceptions/                 # Domain-specific exceptions
│   │
│   └── ArtMarket.Infrastructure/       # Data access & external integrations
│       ├── Data/
│       │   ├── ArtMarketDbContext.cs    # EF Core DbContext
│       │   ├── Configurations/         # Entity type configurations (Fluent API)
│       │   └── Migrations/             # EF Core migration files
│       ├── Repositories/               # IRepository implementations
│       ├── Email/                      # SMTP/SendGrid integration
│       └── Chatbot/                    # OpenAI API client
│
├── tests/
│   ├── ArtMarket.UnitTests/            # Service-level unit tests (xUnit + Moq)
│   └── ArtMarket.IntegrationTests/     # API-level integration tests (WebApplicationFactory)
│
├── docs/
│   ├── images/                         # Logos, diagrams
│   └── screenshots/                    # UI screenshots
│
├── docker-compose.yml                  # Local dev orchestration
├── Dockerfile                          # API container definition
├── .env.example                        # Environment variable template
└── ArtMarket.sln                       # Solution file
```

### Key Folder Responsibilities

| Folder | Responsibility |
|--------|---------------|
| `API/Controllers` | Thin HTTP handlers — no logic, only delegation |
| `API/Middleware` | Cross-cutting concerns: error handling, request logging |
| `Application/Services` | All business rules live here |
| `Application/DTOs` | Shapes exposed to the outside world — never expose domain models |
| `Domain/Entities` | Pure C# classes representing business objects |
| `Domain/Enums` | Typed status values — avoids stringly-typed conditionals |
| `Infrastructure/Repositories` | EF Core queries — one repository per aggregate root |
| `Infrastructure/Email` | SendGrid/SMTP wrapper behind `IEmailService` |
| `Infrastructure/Chatbot` | OpenAI API wrapper behind `IChatbotService` |

---

## 4. Authentication & Authorization Flow

ArtMarket uses **JWT Bearer Token** authentication with role-based authorization.

### Registration & Login Flow

```
1. User registers → POST /api/v1/auth/register
      │
      ▼
2. User record created with hashed password (BCrypt)
   Role assigned: "Buyer" (default) or "Artist" (pending approval)
      │
      ▼
3. If Artist: ArtistStatus = Pending → awaits admin approval
      │
      ▼
4. User logs in → POST /api/v1/auth/login
      │
      ▼
5. Credentials validated → JWT token generated:
   - Claims: UserId, Email, Role, ArtistId (if artist)
   - Signed with HMAC-SHA256 using JwtSettings__SecretKey
   - Expiry: configurable via JwtSettings__ExpiryMinutes
      │
      ▼
6. Token returned in response body → client stores and sends
   in Authorization header: "Bearer {token}"
      │
      ▼
7. Middleware validates token on every protected request
   - Signature verified
   - Expiry checked
   - Role claims extracted and matched to [Authorize] attributes
```

### Role Hierarchy

| Role | Access Level |
|------|-------------|
| `Admin` | Full access — user management, artist approval, platform config |
| `Artist` | Own profile, own artworks, own received orders |
| `Buyer` | Public browsing, placing orders, own order history |

### Token Structure (Decoded Payload)

```json
{
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "artist@example.com",
  "role": "Artist",
  "artistId": "9c84be12-1a22-4f55-9d14-adf123456789",
  "iat": 1716000000,
  "exp": 1716003600,
  "iss": "ArtMarketAPI",
  "aud": "ArtMarketClients"
}
```

### Controller Authorization Examples

```csharp
[Authorize]                        // Any authenticated user
[Authorize(Roles = "Admin")]       // Admin only
[Authorize(Roles = "Artist")]      // Artist only
[Authorize(Roles = "Artist,Admin")]// Artist or Admin
[AllowAnonymous]                   // Public — no token required
```

---

## 5. Subscription & Tier System

Artists operate on one of two tiers:

| Tier | Artwork Limit | Cost |
|------|--------------|------|
| `Free` | 5 listings max | $0 |
| `Premium` | Unlimited | Monthly subscription |

### How It Works

1. When an artist creates a new artwork, `ArtworkService` calls `ISubscriptionService.EnforceListingLimitAsync(artist)`.
2. The subscription service checks:
   - If the artist has a `SubscriptionTier == Premium` → allow (no limit)
   - If `SubscriptionTier == Free` → count existing active listings
   - If count >= `SubscriptionSettings__FreeListingLimit` (default: 5) → throw `ListingLimitExceededException`
3. The controller catches this domain exception and returns `403 Forbidden` with a descriptive message.

### Subscription Entity

```
Subscription
├── Id (Guid)
├── ArtistId (FK → Artist)
├── Tier (Enum: Free | Premium)
├── StartDate (DateTime)
├── EndDate (DateTime?)          ← null = active indefinitely (future payment integration)
└── IsActive (bool)
```

### Upgrade Path (Future)

When Stripe integration is added (Phase 3), the flow will be:
1. Artist initiates upgrade → POST /api/v1/subscriptions/upgrade
2. Stripe Checkout session created
3. Stripe webhook fires on payment success
4. `SubscriptionService` updates `Tier` to `Premium`

---

## 6. Email Notification System

Email notifications are triggered by domain events — primarily when a buyer creates an order request.

### Notification Flow

```
Buyer submits order → POST /api/v1/orders
        │
        ▼
OrderService.CreateOrderAsync()
        │
        ▼
Order saved to database (Status = Pending)
        │
        ▼
IEmailService.SendOrderNotificationAsync(order, artist)
        │ (fire-and-forget using Task.Run or background service)
        ▼
EmailService builds HTML email from template
        │
        ▼
SMTP / SendGrid delivers email to artist's registered address
```

### Email Service Interface

```csharp
public interface IEmailService
{
    Task SendOrderNotificationAsync(Order order, Artist artist, Buyer buyer);
    Task SendArtistApprovalEmailAsync(Artist artist);
    Task SendArtistRejectionEmailAsync(Artist artist, string reason);
    Task SendWelcomeEmailAsync(User user);
}
```

### Email Templates

Email bodies are stored as `.html` files under `ArtMarket.Infrastructure/Email/Templates/`:

| Template | Trigger |
|----------|---------|
| `order-notification.html` | New order request received by artist |
| `artist-approved.html` | Admin approves artist registration |
| `artist-rejected.html` | Admin rejects artist registration |
| `welcome.html` | New user registers successfully |

Templates use `{{TokenName}}` placeholders replaced at runtime by `EmailService`.

---

## 7. Chatbot Integration

The chatbot enables buyers to describe what they're looking for in natural language and receive matched artwork recommendations.

### Architecture

```
Buyer → POST /api/v1/chatbot/message
              │
              ▼
        ChatbotController
              │
              ▼
        IChatbotService
              │
              ▼
        OpenAI API (GPT-4o)
          System Prompt: "You are an art discovery assistant for ArtMarket..."
          User Message: buyer's natural language query
              │
              ▼
        AI responds with structured recommendations
              │
              ▼
        ChatbotService maps recommendations → Artwork IDs from DB
              │
              ▼
        Returns: { message: string, artworkSuggestions: ArtworkSummaryDto[] }
```

### System Prompt Design

The chatbot is seeded with a system prompt that:
- Defines its role as an art discovery assistant
- Instructs it to ask clarifying questions (style, medium, color palette, budget)
- Asks it to return structured JSON with artwork criteria when ready to search
- Avoids hallucinating artwork — it only suggests based on DB query results

### Conversation Context

The API is stateless — each chatbot message includes a `conversationHistory` array in the request body, allowing the client to maintain context across turns without server-side session storage.

---

## 8. Database Entity Relationships

### Entity Relationship Overview (ERD — Text Description)

```
┌─────────────┐       ┌─────────────┐       ┌─────────────────┐
│    User     │──1:1──│   Artist    │──1:N──│    Artwork      │
│─────────────│       │─────────────│       │─────────────────│
│ Id          │       │ Id          │       │ Id              │
│ Email       │       │ UserId (FK) │       │ ArtistId (FK)   │
│ PasswordHash│       │ DisplayName │       │ Title           │
│ Role        │       │ Bio         │       │ Description     │
│ CreatedAt   │       │ Status      │       │ Price           │
└─────────────┘       │ ProfileImage│       │ Medium          │
                      └──────┬──────┘       │ IsAvailable     │
                             │              │ ImageUrl        │
                          1:1│              │ CreatedAt       │
                             ▼              └────────┬────────┘
                      ┌─────────────┐               │
                      │Subscription │               │ 1:N
                      │─────────────│               ▼
                      │ Id          │       ┌─────────────────┐
                      │ ArtistId(FK)│       │     Order       │
                      │ Tier        │       │─────────────────│
                      │ IsActive    │       │ Id              │
                      │ StartDate   │       │ ArtworkId (FK)  │
                      │ EndDate     │       │ BuyerId (FK)    │
                      └─────────────┘       │ ArtistId (FK)   │
                                            │ Message         │
                      ┌─────────────┐       │ Status          │
                      │    Buyer    │──1:N──│ CreatedAt       │
                      │─────────────│       └─────────────────┘
                      │ Id          │
                      │ UserId (FK) │
                      │ DisplayName │
                      └─────────────┘
```

### Entity Descriptions

#### `User`
Base identity record. Stores credentials and role. One-to-one with either `Artist` or `Buyer` depending on role.

#### `Artist`
Extended profile for artist users. Has a `Status` enum: `Pending | Approved | Rejected`. Only `Approved` artists have visible listings.

#### `Buyer`
Extended profile for buyer users. Minimal — primarily used to associate orders with identities.

#### `Artwork`
Represents a single listing by an artist. Linked to `Artist`. Has availability and soft-delete flags.

#### `Order`
Represents a buyer's request for an artwork. Links `Buyer`, `Artist`, and `Artwork`. Has a `Status` enum: `Pending | Accepted | Declined | Completed`.

#### `Subscription`
Tracks an artist's current tier. `Tier` enum: `Free | Premium`. Used by `SubscriptionService` to enforce listing limits.

### Key Constraints

- An `Artwork` belongs to exactly one `Artist`
- An `Order` references an `Artwork`, but also denormalizes `ArtistId` for efficient notification lookup
- `Artist.Status` must be `Approved` before their artworks appear in public listings
- Soft deletes are used on `Artwork` (via `IsDeleted` flag) so order history remains intact
