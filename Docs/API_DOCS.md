# API_DOCS.md — ArtMarket API Reference

> Full reference for all ArtMarket API endpoints. For authentication setup, see the [Auth section](#authentication).

---

## Table of Contents

1. [Base URL & Versioning](#1-base-url--versioning)
2. [Authentication](#2-authentication)
3. [Error Format](#3-error-format)
4. [Auth Endpoints](#4-auth-endpoints)
5. [Admin Endpoints](#5-admin-endpoints)
6. [Artist Endpoints](#6-artist-endpoints)
7. [Artwork Endpoints](#7-artwork-endpoints)
8. [Buyer Endpoints](#8-buyer-endpoints)
9. [Order Endpoints](#9-order-endpoints)
10. [Chatbot Endpoints](#10-chatbot-endpoints)

---

## 1. Base URL & Versioning

```
Base URL:    https://api.artmarket.io/api/v1
Dev URL:     https://localhost:7001/api/v1
```

All endpoints are versioned under `/api/v1/`. The version is included in the URL path to allow non-breaking evolution.

---

## 2. Authentication

ArtMarket uses **JWT Bearer Token** authentication. All protected endpoints require the following header:

```
Authorization: Bearer <your_jwt_token>
```

Obtain a token by calling `POST /api/v1/auth/login`. Tokens expire after the configured duration (default: 60 minutes).

### Access Matrix

| Role | Prefix | Notes |
|------|--------|-------|
| `Public` | No token needed | Browse artworks, artist profiles |
| `Buyer` | Any authenticated non-artist user | Place orders, use chatbot |
| `Artist` | Artists with `Status = Approved` | Manage listings, view received orders |
| `Admin` | Platform administrators | Full access |

---

## 3. Error Format

All errors return a consistent JSON structure:

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    "Title is required",
    "Price must be greater than 0"
  ],
  "traceId": "00-4af6...abc-01"
}
```

### Common Status Codes

| Code | Meaning |
|------|---------|
| `200` | OK |
| `201` | Created |
| `204` | No Content |
| `400` | Bad Request — validation error |
| `401` | Unauthorized — missing or invalid token |
| `403` | Forbidden — authenticated but lacks permission |
| `404` | Not Found |
| `409` | Conflict — e.g., duplicate email |
| `422` | Unprocessable — business rule violation (e.g., listing limit reached) |
| `500` | Internal Server Error |

---

## 4. Auth Endpoints

### Register

**POST** `/api/v1/auth/register`

Creates a new user account. Defaults to `Buyer` role unless `"role": "Artist"` is specified.

**Access:** Public

**Request Body:**
```json
{
  "displayName": "Layla Hassan",
  "email": "layla@example.com",
  "password": "SecurePass123!",
  "role": "Artist"
}
```

**Response `201 Created`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "displayName": "Layla Hassan",
  "email": "layla@example.com",
  "role": "Artist",
  "artistStatus": "Pending",
  "message": "Registration successful. Your artist account is pending admin approval."
}
```

**Status Codes:** `201`, `400`, `409`

---

### Login

**POST** `/api/v1/auth/login`

Authenticates a user and returns a JWT token.

**Access:** Public

**Request Body:**
```json
{
  "email": "layla@example.com",
  "password": "SecurePass123!"
}
```

**Response `200 OK`:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-15T15:00:00Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "displayName": "Layla Hassan",
    "email": "layla@example.com",
    "role": "Artist"
  }
}
```

**Status Codes:** `200`, `400`, `401`

---

### Refresh Token

**POST** `/api/v1/auth/refresh`

Issues a new JWT token using a valid (non-expired) existing token.

**Access:** Any authenticated user

**Request Body:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response `200 OK`:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...(new)...",
  "expiresAt": "2025-01-15T16:00:00Z"
}
```

**Status Codes:** `200`, `401`

---

### Get Current User

**GET** `/api/v1/auth/me`

Returns the profile of the currently authenticated user.

**Access:** Any authenticated user

**Response `200 OK`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "displayName": "Layla Hassan",
  "email": "layla@example.com",
  "role": "Artist",
  "artistStatus": "Approved"
}
```

**Status Codes:** `200`, `401`

---

## 5. Admin Endpoints

All admin endpoints require `Authorization: Bearer <admin_token>`.

### List Pending Artists

**GET** `/api/v1/admin/artists/pending`

Returns all artist accounts awaiting approval.

**Access:** Admin only

**Response `200 OK`:**
```json
[
  {
    "id": "9c84be12-1a22-4f55-9d14-adf123456789",
    "displayName": "Omar Farouk",
    "email": "omar@example.com",
    "bio": "Watercolor artist from Cairo.",
    "registeredAt": "2025-01-10T09:00:00Z"
  }
]
```

**Status Codes:** `200`, `401`, `403`

---

### Approve Artist

**POST** `/api/v1/admin/artists/{artistId}/approve`

Approves a pending artist. Sends an approval email to the artist.

**Access:** Admin only

**Response `200 OK`:**
```json
{
  "artistId": "9c84be12-1a22-4f55-9d14-adf123456789",
  "status": "Approved",
  "message": "Artist has been approved and notified by email."
}
```

**Status Codes:** `200`, `401`, `403`, `404`, `409`

---

### Reject Artist

**POST** `/api/v1/admin/artists/{artistId}/reject`

Rejects a pending artist registration.

**Access:** Admin only

**Request Body:**
```json
{
  "reason": "Profile information is incomplete. Please reapply with a full bio and portfolio."
}
```

**Response `200 OK`:**
```json
{
  "artistId": "9c84be12-1a22-4f55-9d14-adf123456789",
  "status": "Rejected",
  "message": "Artist has been rejected and notified."
}
```

**Status Codes:** `200`, `400`, `401`, `403`, `404`

---

### List All Users

**GET** `/api/v1/admin/users`

Returns a paginated list of all platform users.

**Access:** Admin only

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |
| `role` | string | null | Filter by role: `Admin`, `Artist`, `Buyer` |

**Response `200 OK`:**
```json
{
  "data": [
    {
      "id": "3fa85f64-...",
      "displayName": "Layla Hassan",
      "email": "layla@example.com",
      "role": "Artist",
      "artistStatus": "Approved",
      "createdAt": "2025-01-05T10:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 142,
  "totalPages": 8
}
```

**Status Codes:** `200`, `401`, `403`

---

## 6. Artist Endpoints

### Get Artist Profile

**GET** `/api/v1/artists/{artistId}`

Returns the public profile of an approved artist including their active listings.

**Access:** Public

**Response `200 OK`:**
```json
{
  "id": "9c84be12-1a22-4f55-9d14-adf123456789",
  "displayName": "Omar Farouk",
  "bio": "Watercolor artist from Cairo, Egypt.",
  "profileImageUrl": "https://cdn.artmarket.io/profiles/omar.jpg",
  "subscriptionTier": "Premium",
  "approvedAt": "2025-01-11T12:00:00Z",
  "artworks": [
    {
      "id": "ab12cd34-...",
      "title": "Cairo at Sunset",
      "price": 450.00,
      "medium": "Watercolor",
      "thumbnailUrl": "https://cdn.artmarket.io/artworks/thumb/cairo-sunset.jpg",
      "isAvailable": true
    }
  ]
}
```

**Status Codes:** `200`, `404`

---

### Update Artist Profile

**PUT** `/api/v1/artists/me`

Updates the authenticated artist's own profile.

**Access:** Artist (Approved)

**Request Body:**
```json
{
  "displayName": "Omar Farouk",
  "bio": "Watercolor and oil artist based in Cairo.",
  "profileImageUrl": "https://cdn.artmarket.io/profiles/omar-v2.jpg"
}
```

**Response `200 OK`:**
```json
{
  "id": "9c84be12-...",
  "displayName": "Omar Farouk",
  "bio": "Watercolor and oil artist based in Cairo.",
  "profileImageUrl": "https://cdn.artmarket.io/profiles/omar-v2.jpg",
  "updatedAt": "2025-01-15T14:00:00Z"
}
```

**Status Codes:** `200`, `400`, `401`, `403`

---

### Get My Artworks

**GET** `/api/v1/artists/me/artworks`

Returns all artworks (including unavailable) for the authenticated artist.

**Access:** Artist (Approved)

**Response `200 OK`:**
```json
[
  {
    "id": "ab12cd34-...",
    "title": "Cairo at Sunset",
    "description": "A watercolor scene of the Nile at dusk.",
    "price": 450.00,
    "medium": "Watercolor",
    "dimensions": "40cm x 60cm",
    "imageUrl": "https://cdn.artmarket.io/artworks/cairo-sunset.jpg",
    "isAvailable": true,
    "createdAt": "2025-01-12T10:00:00Z"
  }
]
```

**Status Codes:** `200`, `401`, `403`

---

### Get Received Orders

**GET** `/api/v1/artists/me/orders`

Returns all order requests received by the authenticated artist.

**Access:** Artist (Approved)

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `status` | string | null | Filter: `Pending`, `Accepted`, `Declined`, `Completed` |
| `page` | int | 1 | Page number |

**Response `200 OK`:**
```json
{
  "data": [
    {
      "id": "order-uuid-here",
      "artwork": {
        "id": "ab12cd34-...",
        "title": "Cairo at Sunset"
      },
      "buyer": {
        "id": "buyer-uuid-here",
        "displayName": "Sara Ali",
        "email": "sara@example.com"
      },
      "message": "Hi! I'd love to purchase this piece. Can we discuss shipping?",
      "status": "Pending",
      "createdAt": "2025-01-15T11:00:00Z"
    }
  ],
  "page": 1,
  "totalCount": 7
}
```

**Status Codes:** `200`, `401`, `403`

---

## 7. Artwork Endpoints

### List Artworks (Public)

**GET** `/api/v1/artworks`

Returns a paginated list of available artworks from approved artists.

**Access:** Public

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 12 | Items per page |
| `medium` | string | null | Filter by medium (e.g., `Watercolor`, `Oil`, `Digital`) |
| `minPrice` | decimal | null | Minimum price filter |
| `maxPrice` | decimal | null | Maximum price filter |
| `search` | string | null | Search in title and description |
| `sortBy` | string | `newest` | `newest`, `price_asc`, `price_desc` |

**Response `200 OK`:**
```json
{
  "data": [
    {
      "id": "ab12cd34-...",
      "title": "Cairo at Sunset",
      "price": 450.00,
      "medium": "Watercolor",
      "thumbnailUrl": "https://cdn.artmarket.io/artworks/thumb/cairo-sunset.jpg",
      "artist": {
        "id": "9c84be12-...",
        "displayName": "Omar Farouk",
        "profileImageUrl": "https://cdn.artmarket.io/profiles/omar.jpg"
      },
      "isAvailable": true,
      "createdAt": "2025-01-12T10:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 12,
  "totalCount": 87,
  "totalPages": 8
}
```

**Status Codes:** `200`

---

### Get Artwork Detail

**GET** `/api/v1/artworks/{artworkId}`

Returns full details for a single artwork.

**Access:** Public

**Response `200 OK`:**
```json
{
  "id": "ab12cd34-...",
  "title": "Cairo at Sunset",
  "description": "A watercolor scene of the Nile at dusk, painted en plein air.",
  "price": 450.00,
  "medium": "Watercolor",
  "dimensions": "40cm x 60cm",
  "imageUrl": "https://cdn.artmarket.io/artworks/cairo-sunset.jpg",
  "isAvailable": true,
  "artist": {
    "id": "9c84be12-...",
    "displayName": "Omar Farouk",
    "profileImageUrl": "https://cdn.artmarket.io/profiles/omar.jpg",
    "bio": "Watercolor artist from Cairo."
  },
  "createdAt": "2025-01-12T10:00:00Z"
}
```

**Status Codes:** `200`, `404`

---

### Create Artwork

**POST** `/api/v1/artworks`

Creates a new artwork listing for the authenticated artist.

**Access:** Artist (Approved)

> ⚠️ Free-tier artists are limited to 5 active listings. Exceeding this returns `422`.

**Request Body:**
```json
{
  "title": "Desert Storm",
  "description": "An abstract representation of a sandstorm in the Sahara.",
  "price": 320.00,
  "medium": "Oil on Canvas",
  "dimensions": "50cm x 70cm",
  "imageUrl": "https://cdn.artmarket.io/artworks/desert-storm.jpg"
}
```

**Response `201 Created`:**
```json
{
  "id": "new-artwork-uuid",
  "title": "Desert Storm",
  "price": 320.00,
  "medium": "Oil on Canvas",
  "isAvailable": true,
  "createdAt": "2025-01-15T14:30:00Z"
}
```

**Status Codes:** `201`, `400`, `401`, `403`, `422`

---

### Update Artwork

**PUT** `/api/v1/artworks/{artworkId}`

Updates an existing artwork owned by the authenticated artist.

**Access:** Artist (owner only)

**Request Body:**
```json
{
  "title": "Desert Storm II",
  "price": 380.00,
  "isAvailable": false
}
```

**Response `200 OK`:** *(updated artwork object)*

**Status Codes:** `200`, `400`, `401`, `403`, `404`

---

### Delete Artwork

**DELETE** `/api/v1/artworks/{artworkId}`

Soft-deletes an artwork. The record is retained for order history.

**Access:** Artist (owner only)

**Response `204 No Content`**

**Status Codes:** `204`, `401`, `403`, `404`

---

## 8. Buyer Endpoints

### Get Buyer Profile

**GET** `/api/v1/buyers/me`

Returns the authenticated buyer's profile and order history summary.

**Access:** Buyer

**Response `200 OK`:**
```json
{
  "id": "buyer-uuid-here",
  "displayName": "Sara Ali",
  "email": "sara@example.com",
  "totalOrders": 3,
  "createdAt": "2025-01-01T08:00:00Z"
}
```

**Status Codes:** `200`, `401`, `403`

---

### Get My Orders

**GET** `/api/v1/buyers/me/orders`

Returns all orders placed by the authenticated buyer.

**Access:** Buyer

**Query Parameters:** `status`, `page`, `pageSize`

**Response `200 OK`:**
```json
{
  "data": [
    {
      "id": "order-uuid-here",
      "artwork": {
        "id": "ab12cd34-...",
        "title": "Cairo at Sunset",
        "thumbnailUrl": "https://cdn.artmarket.io/artworks/thumb/cairo-sunset.jpg"
      },
      "artist": {
        "id": "9c84be12-...",
        "displayName": "Omar Farouk",
        "profileUrl": "/artists/9c84be12-..."
      },
      "status": "Pending",
      "createdAt": "2025-01-15T11:00:00Z"
    }
  ],
  "page": 1,
  "totalCount": 3
}
```

**Status Codes:** `200`, `401`, `403`

---

## 9. Order Endpoints

### Create Order Request

**POST** `/api/v1/orders`

Submits an order request for an artwork. Triggers an email notification to the artist. Returns a redirect URL to the artist's profile.

**Access:** Buyer

**Request Body:**
```json
{
  "artworkId": "ab12cd34-ef56-7890-abcd-ef1234567890",
  "message": "Hi Omar! I love this piece. I'm based in Alexandria — can we discuss delivery options?"
}
```

**Response `201 Created`:**
```json
{
  "orderId": "order-uuid-here",
  "status": "Pending",
  "message": "Your request has been sent. The artist will contact you by email.",
  "artistProfileUrl": "/artists/9c84be12-1a22-4f55-9d14-adf123456789",
  "createdAt": "2025-01-15T11:00:00Z"
}
```

> 💡 **Note:** The `artistProfileUrl` is the URL the buyer should be redirected to. The actual deal (price negotiation, shipping arrangements) happens directly between the buyer and artist through the artist's profile or via email.

**Status Codes:** `201`, `400`, `401`, `403`, `404`, `409`

---

### Get Order Detail

**GET** `/api/v1/orders/{orderId}`

Returns full details for a specific order.

**Access:** Buyer (order owner) or Artist (recipient) or Admin

**Response `200 OK`:**
```json
{
  "id": "order-uuid-here",
  "artwork": {
    "id": "ab12cd34-...",
    "title": "Cairo at Sunset",
    "price": 450.00,
    "imageUrl": "https://cdn.artmarket.io/artworks/cairo-sunset.jpg"
  },
  "buyer": {
    "id": "buyer-uuid-here",
    "displayName": "Sara Ali",
    "email": "sara@example.com"
  },
  "artist": {
    "id": "9c84be12-...",
    "displayName": "Omar Farouk"
  },
  "message": "Hi Omar! I love this piece.",
  "status": "Pending",
  "createdAt": "2025-01-15T11:00:00Z",
  "updatedAt": "2025-01-15T11:00:00Z"
}
```

**Status Codes:** `200`, `401`, `403`, `404`

---

### Update Order Status

**PATCH** `/api/v1/orders/{orderId}/status`

Allows the artist to accept, decline, or mark an order as completed.

**Access:** Artist (recipient of the order)

**Request Body:**
```json
{
  "status": "Accepted"
}
```

Valid status transitions: `Pending → Accepted`, `Pending → Declined`, `Accepted → Completed`

**Response `200 OK`:**
```json
{
  "orderId": "order-uuid-here",
  "status": "Accepted",
  "updatedAt": "2025-01-15T14:00:00Z"
}
```

**Status Codes:** `200`, `400`, `401`, `403`, `404`, `422`

---

## 10. Chatbot Endpoints

### Send Message to Chatbot

**POST** `/api/v1/chatbot/message`

Sends a message to the AI art discovery assistant. The chatbot helps buyers find artwork that matches their description, style preferences, or budget.

**Access:** Any authenticated user (Buyer recommended)

**Request Body:**
```json
{
  "message": "I'm looking for something abstract and colorful, under $300, to hang in my living room.",
  "conversationHistory": [
    {
      "role": "user",
      "content": "Hello, I need help finding art."
    },
    {
      "role": "assistant",
      "content": "I'd be happy to help! What style of art are you drawn to?"
    }
  ]
}
```

> 💡 `conversationHistory` is optional on the first message. Include previous turns to maintain context.

**Response `200 OK`:**
```json
{
  "reply": "I found a few abstract pieces that might work perfectly for your space! Here are some options:",
  "artworkSuggestions": [
    {
      "id": "artwork-uuid-1",
      "title": "Vibrant Chaos",
      "price": 250.00,
      "medium": "Acrylic",
      "thumbnailUrl": "https://cdn.artmarket.io/artworks/thumb/vibrant-chaos.jpg",
      "artist": {
        "id": "artist-uuid-1",
        "displayName": "Nour Khalil"
      }
    },
    {
      "id": "artwork-uuid-2",
      "title": "Blue Harmony",
      "price": 280.00,
      "medium": "Mixed Media",
      "thumbnailUrl": "https://cdn.artmarket.io/artworks/thumb/blue-harmony.jpg",
      "artist": {
        "id": "artist-uuid-2",
        "displayName": "Amira Saad"
      }
    }
  ]
}
```

**Status Codes:** `200`, `400`, `401`, `500`

---

### Get Chatbot Status

**GET** `/api/v1/chatbot/status`

Returns the operational status of the chatbot service.

**Access:** Public

**Response `200 OK`:**
```json
{
  "status": "Operational",
  "model": "gpt-4o",
  "checkedAt": "2025-01-15T14:00:00Z"
}
```

**Status Codes:** `200`, `503`