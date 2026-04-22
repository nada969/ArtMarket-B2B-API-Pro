# Art Marketplace — API Documentation

> **Version:** 1.0.0  
> **Base URL:** `https://api.artmarketplace.com/api`  
> **Auth:** Bearer JWT Token (all protected routes require `Authorization: Bearer <token>`)

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Data Model](#data-model)
3. [Authentication](#authentication)
4. [Users & Profiles](#users--profiles)
5. [Artworks](#artworks)
6. [Orders](#orders)
7. [Subscriptions](#subscriptions)
8. [Admin](#admin)
9. [Error Handling](#error-handling)
10. [Design Decisions & Reasoning](#design-decisions--reasoning)

---

## Architecture Overview

```
ArtMarketplace/
├── ArtMarketplace.API/                  # Presentation layer (Controllers, Middleware)
├── ArtMarketplace.Application/          # Business logic (Services, DTOs, Interfaces)
├── ArtMarketplace.Domain/               # Core entities & Enums
└── ArtMarketplace.Infrastructure/       # DB, Repositories, Email
```

**Pattern:** Clean Architecture (Domain → Application → Infrastructure → API)  
**Auth:** JWT Bearer Tokens  
**Database:** SQL Server + Entity Framework Core  
**Email:** Triggered server-side on status changes (not by client endpoints)

---

## Data Model

> **V1 decision:** One order = one artwork directly. No `OrderItems` table yet. Designed so migration to multi-item orders requires only an internal model change — no endpoint changes.

### Entity Relationship Diagram

```
User (IdentityUser) (1) ──── (0..1) Artist
User (IdentityUser) (1) ──── (0..1) Buyer
Artist             (1) ──── (0..1) Subscription
Artist             (1) ──── (0..n) Artworks
Artist             (1) ──── (0..n) Orders       [incoming orders — denormalized FK]
Buyer              (1) ──── (0..n) Orders       [placed orders]
Artwork            (1) ──── (0..n) Orders       [V1: direct FK on order]
```

### C# Domain Entities (from source)

**User** — extends `IdentityUser` (handles email, password hash, claims via ASP.NET Identity)
```csharp
public class User : IdentityUser
{
    public UserRole Role { get; set; }      // Enum: Artist | Buyer | Admin
    public DateTime CreatedAt { get; set; }
    public Buyer? Buyer { get; set; }       // null if role is Artist
    public Artist? Artist { get; set; }     // null if role is Buyer
}
```

**Artist** — profile entity, linked 1-to-1 with User
```csharp
public class Artist
{
    public string ArtistId { get; set; }
    public string ArtistDisplayName { get; set; }
    public string Bio { get; set; }
    public Status Status { get; set; }      // Enum: e.g. Active | Suspended | Pending
    public string ProfileImage { get; set; }
    // FK to User
    public string UserId { get; set; }
    public User User { get; set; }
    // Navigation
    public Subscription? Subscription { get; set; }
    public ICollection<Artwork> Artworks { get; set; }
    public ICollection<Order> Orders { get; set; }
}
```

**Buyer** — profile entity, linked 1-to-1 with User
```csharp
public class Buyer
{
    public string BuyerId { get; set; }
    public string BuyerDisplayName { get; set; }
    // FK to User
    public string UserId { get; set; }
    public User User { get; set; }
    // Navigation
    public ICollection<Order> Orders { get; set; }
}
```

**Artwork** — belongs to Artist, directly referenced by Orders in V1
```csharp
public class Artwork
{
    public string ArtId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public bool IsAvailable { get; set; }   // true = can be ordered, false = sold/hidden
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    // FK
    [ForeignKey("Artist")]
    public string ArtistId { get; set; }
    public Artist Artist { get; set; }
    // Navigation
    public ICollection<Order> Orders { get; set; }
}
```

**Order** — V1: holds direct FKs to Artwork, Buyer, and Artist
```csharp
public class Order
{
    public string OrderId { get; set; }
    public Status Status { get; set; }      // Enum: Pending | Approved | Declined | Cancelled
    // FKs
    [ForeignKey("ArtWork")]
    public string ArtId { get; set; }
    public Artwork ArtWork { get; set; }

    [ForeignKey("Buyer")]
    public string BuyerId { get; set; }
    public Buyer Buyer { get; set; }

    [ForeignKey("Artist")]
    public string ArtistId { get; set; }    // denormalized from ArtWork.ArtistId for query convenience
    public Artist Artist { get; set; }
}
```

> ⚠️ **Note on `ArtistId` in Order:** It is derived from `ArtWork.ArtistId` and must be set on creation. This denormalization is acceptable in V1 — it makes `GET /orders?artist_id=x` a simple filter without a join. In V2 with `OrderItems`, this field would be removed.

**Subscription** — belongs to Artist, tracks platform access tier
```csharp
public class Subscription
{
    public string SubscriptionId { get; set; }
    public SubscriptionTier Tier { get; set; }  // Enum: Basic | Pro
    public bool IsActive { get; set; }           // true = currently valid
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    // FK
    public string ArtistId { get; set; }
    public Artist Artist { get; set; }
}
```

---

## Authentication

All auth endpoints are public (no token required).

### POST `/auth/register`

Register a new user (artist or buyer).

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "role": "buyer"
}
```

**Response `201 Created`:**
```json
{
  "id": "uuid",
  "email": "user@example.com",
  "role": "buyer",
  "createdAt": "2025-01-01T00:00:00Z"
}
```

---

### POST `/auth/login`

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response `200 OK`:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "abc123...",
  "expiresIn": 3600
}
```

---

### POST `/auth/logout`

Invalidates refresh token. Requires auth.

**Response `204 No Content`**

---

### POST `/auth/password/forgot`

Sends password reset email.

**Request Body:**
```json
{ "email": "user@example.com" }
```

**Response `200 OK`:**
```json
{ "message": "If this email exists, a reset link has been sent." }
```

---

### POST `/auth/password/reset`

**Request Body:**
```json
{
  "token": "reset-token-from-email",
  "newPassword": "NewSecurePass123!"
}
```

**Response `204 No Content`**

---

## Users & Profiles

### GET `/users/me` 🔒

Returns the current authenticated user with their profile (Artist or Buyer depending on role).

**Response `200 OK` (Artist):**
```json
{
  "id": "aspnet-identity-id",
  "email": "artist@example.com",
  "role": "Artist",
  "createdAt": "2025-01-01T00:00:00Z",
  "profile": {
    "artistId": "uuid",
    "displayName": "Ahmed Hassan",
    "bio": "Contemporary digital artist",
    "profileImage": "https://cdn.example.com/images/ahmed.jpg",
    "status": "Active"
  }
}
```

**Response `200 OK` (Buyer):**
```json
{
  "id": "aspnet-identity-id",
  "email": "buyer@example.com",
  "role": "Buyer",
  "createdAt": "2025-01-01T00:00:00Z",
  "profile": {
    "buyerId": "uuid",
    "displayName": "Sara Ali"
  }
}
```

---

### PATCH `/users/me` 🔒

Update current user's profile fields. Only fields relevant to the user's role are accepted.

**Request Body (Artist):**
```json
{
  "displayName": "Ahmed Hassan",
  "bio": "Updated bio",
  "profileImage": "https://..."
}
```

**Request Body (Buyer):**
```json
{
  "displayName": "Sara Ali"
}
```

**Response `200 OK`:** Updated profile object.

---

### GET `/artists/{artistId}`

Public artist profile page.

**Response `200 OK`:**
```json
{
  "artistId": "uuid",
  "displayName": "Ahmed Hassan",
  "bio": "Abstract expressionist",
  "profileImage": "https://...",
  "status": "Active",
  "subscription": {
    "tier": "Pro",
    "isActive": true
  }
}
```

---

### GET `/artists/{artistId}/artworks`

Get all available artworks for a specific artist (public gallery).

**Query Params:** `page`, `limit`, `sort`

---

## Artworks

### GET `/artworks`

List artworks with filtering and sorting.

**Query Parameters:**

| Param | Type | Description |
|-------|------|-------------|
| `artist_id` | string | Filter by artist |
| `is_available` | bool | `true` (default) or `false` |
| `sort` | string | `price_asc`, `price_desc`, `newest` |
| `page` | int | Page number (default: 1) |
| `limit` | int | Per page (default: 20, max: 100) |

**Example:** `GET /artworks?artist_id=abc&sort=price_asc&page=1`

**Response `200 OK`:**
```json
{
  "data": [
    {
      "artId": "uuid",
      "title": "Blue Horizon",
      "description": "...",
      "price": 450.00,
      "isAvailable": true,
      "imageUrl": "https://cdn.example.com/art/blue-horizon.jpg",
      "createdAt": "2025-01-01T00:00:00Z",
      "artist": {
        "artistId": "uuid",
        "displayName": "Ahmed Hassan",
        "profileImage": "https://..."
      }
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 87,
    "totalPages": 5
  }
}
```

---

### POST `/artworks` 🔒 (Artist only)

Create a new artwork. `isAvailable` defaults to `true`. `createdAt` is set server-side.

**Request Body:**
```json
{
  "title": "Desert Sunset",
  "description": "An oil painting of the Egyptian desert",
  "price": 1200.00,
  "imageUrl": "https://cdn.example.com/art/desert-sunset.jpg"
}
```

**Response `201 Created`:** Full artwork object.

---

### GET `/artworks/{artId}`

Get a single artwork by its `ArtId`.

**Response `200 OK`:** Full artwork object including artist display name and profile image.

---

### PUT `/artworks/{artId}` 🔒 (Owner artist only)

Full replacement update of an artwork. All fields required.

---

### PATCH `/artworks/{artId}` 🔒 (Owner artist only)

Partial update. Send only the fields to change.

**Request Body:**
```json
{
  "price": 1350.00,
  "isAvailable": false
}
```

**Response `200 OK`:** Updated artwork object.

---

### DELETE `/artworks/{artId}` 🔒 (Owner artist only)

Delete an artwork. Blocked if the artwork has any non-cancelled orders.

**Response `204 No Content`**

---

## Orders

> **Key design decision:** Orders are top-level resources filtered by query parameters — not deeply nested under buyers or artists. This is the industry standard (Etsy, Shopify, Amazon).
>
> **V1 model:** One order = one artwork. `ArtistId` is stored directly on the order (denormalized from the artwork) for efficient filtering of an artist's incoming orders.

### GET `/orders` 🔒

List orders. Results are scoped automatically based on the caller's role.

**Query Parameters:**

| Param | Type | Description |
|-------|------|-------------|
| `buyer_id` | string | Filter by buyer (admin use) |
| `artist_id` | string | Filter by artist — their incoming orders |
| `status` | string | `Pending`, `Approved`, `Declined`, `Cancelled` |
| `page` | int | Page number |
| `limit` | int | Per page |

**Examples:**
```
GET /orders?artist_id=abc    → artist sees their inbox
GET /orders?buyer_id=xyz     → buyer sees their history
GET /orders?status=Pending   → all pending orders (admin)
```

---

### POST `/orders` 🔒 (Buyer only)

Place a new order for a single artwork (V1).

**Request Body:**
```json
{
  "artId": "uuid-of-artwork"
}
```

**Response `201 Created`:**
```json
{
  "orderId": "uuid",
  "status": "Pending",
  "artwork": {
    "artId": "uuid",
    "title": "Blue Horizon",
    "price": 450.00,
    "imageUrl": "https://..."
  },
  "artist": {
    "artistId": "uuid",
    "displayName": "Ahmed Hassan"
  },
  "buyer": {
    "buyerId": "uuid",
    "displayName": "Sara Ali"
  }
}
```

> 🔔 **Server-side:** Artist is automatically notified by email when a new order is placed.

---

### GET `/orders/{orderId}` 🔒

Get a specific order. Only accessible by the buyer who placed it, the artist receiving it, or an admin.

**Response `200 OK`:** Full order object as shown above.

---

### PATCH `/orders/{orderId}` 🔒

Update order status. Follows a strict state machine — invalid transitions are rejected with `422`.

**Request Body:**
```json
{ "status": "Approved" }
```

**Valid status transitions:**

| From | To | Who |
|------|----|-----|
| `Pending` | `Approved` | Artist |
| `Pending` | `Declined` | Artist |
| `Pending` | `Cancelled` | Buyer |
| `Approved` | `Cancelled` | Artist / Admin |

> 🔔 **Server-side:** Buyer is automatically notified by email on `Approved` or `Declined`.
> Setting `Approved` also flips `Artwork.IsAvailable` to `false` server-side.

**Response `204 No Content`**

---

### DELETE `/orders/{orderId}` 🔒

Hard delete. Only allowed if status is `Pending` or `Cancelled`.

**Response `204 No Content`**

---

## Subscriptions

Artists must have an active subscription to list artworks on the platform.

### GET `/subscriptions/me` 🔒 (Artist only)

Get the current artist's subscription.

**Response `200 OK`:**
```json
{
  "subscriptionId": "uuid",
  "tier": "Pro",
  "isActive": true,
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2026-01-01T00:00:00Z"
}
```

**Response `404 Not Found`** if artist has no subscription yet.

---

### POST `/subscriptions` 🔒 (Artist only)

Submit a new subscription request. Sets `isActive: false` until admin approves.

**Request Body:**
```json
{ "tier": "Pro" }
```

**Response `201 Created`:**
```json
{
  "subscriptionId": "uuid",
  "tier": "Pro",
  "isActive": false,
  "startDate": null,
  "endDate": null
}
```

---

### PATCH `/subscriptions/{subscriptionId}` 🔒 (Admin only)

Activate or deactivate a subscription. On activation, `startDate` and `endDate` are set server-side.

**Request Body:**
```json
{ "isActive": true }
```

**Response `204 No Content`**

---

## Admin

### GET `/admin/users` 🔒 (Admin only)

List all users with optional filters.

**Query Params:** `role`, `is_approved`, `page`, `limit`

---

### PATCH `/admin/users/{userId}` 🔒 (Admin only)

Update a user's account status or role.

**Request Body:**
```json
{
  "isApproved": true,
  "role": "artist"
}
```

**Response `200 OK`:** Updated user object.

---

## Error Handling

All errors follow a consistent structure:

```json
{
  "status": 404,
  "error": "Not Found",
  "message": "Artwork with ID 'abc' was not found.",
  "traceId": "0HN7G6..."
}
```

### HTTP Status Codes Used

| Code | Meaning |
|------|---------|
| `200` | OK |
| `201` | Created |
| `204` | No Content (success with no body) |
| `400` | Bad Request (validation error) |
| `401` | Unauthorized (missing/invalid token) |
| `403` | Forbidden (valid token, wrong role/ownership) |
| `404` | Not Found |
| `409` | Conflict (e.g., duplicate email) |
| `422` | Unprocessable Entity (business rule violation) |
| `500` | Internal Server Error |

---

## Design Decisions & Reasoning

### Why are orders top-level, not nested under buyers or artists?

An order has **two stakeholders**: the buyer who placed it, and the artist who fulfills it. Nesting it under either one would mean the other can't access it cleanly. The industry solution (used by Etsy, Shopify, Amazon) is to make orders a **first-class resource** and use query parameters to filter by perspective:

```
GET /orders?buyer_id=xyz   → buyer's history
GET /orders?artist_id=abc  → artist's inbox
```

### Why is ArtistId stored directly on Order in V1?

`Order.ArtistId` is technically derivable via `Order.ArtWork.ArtistId`, but storing it directly avoids a JOIN every time an artist queries their inbox. This is a deliberate **denormalization tradeoff** acceptable in V1. The risk: if an artwork's artist somehow changes, the order becomes inconsistent — mitigated by never allowing artist reassignment on artworks.

### Why no OrderItems in V1?

This is a YAGNI (You Aren't Gonna Need It) decision. One order = one artwork is sufficient for the first release. The endpoint contract (`POST /orders` with `artId`) is designed so that V2 migration only changes the internal model and request body — no URL changes needed.

### Why does email notification not have its own endpoint?

Email is a **side effect of a state change**, not an independent API action. When order status changes to `Approved`, the server fires the notification automatically. Exposing `POST /orders/{id}/mail` would let clients trigger emails arbitrarily, breaking consistency and opening abuse vectors.

### Why does User extend IdentityUser instead of being a plain class?

ASP.NET Identity provides authentication, password hashing, claims, roles, and token management out of the box. Extending `IdentityUser` means you get all of that for free. `Artist` and `Buyer` are **separate profile tables** linked by `UserId` — this keeps identity concerns separate from domain concerns, and allows one person to hold both roles in the future.

### Why use PATCH for status changes instead of action URLs?

```
❌ POST /orders/{id}/approve   ← verb in URL, not RESTful
✅ PATCH /orders/{id}  body: { "status": "Approved" }  ← state machine via body
```

The order **resource** is being mutated. `PATCH` is the correct verb. This also means adding a new status (`Refunded`, `Disputed`) requires zero endpoint changes.

---

*Art Marketplace API v1.0.0 — updated to reflect actual domain models*