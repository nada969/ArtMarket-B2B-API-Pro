# CHANGELOG

All notable changes to ArtMarket are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

> Changes that are merged to `main` but not yet tagged for release.

---

## [1.0.0] — 2025-01-15

> Initial production release of the ArtMarket platform. This release delivers the full MVP feature set: artist onboarding, tiered listings, buyer order requests, AI-powered art discovery, and email notifications.

### Added

#### Authentication & Authorization
- JWT Bearer Token authentication using HMAC-SHA256 signing
- Role-based authorization with three roles: `Admin`, `Artist`, `Buyer`
- User registration endpoint supporting both Artist and Buyer registration
- User login endpoint returning signed JWT with role claims
- Token refresh endpoint for extending session without re-login
- `GET /api/v1/auth/me` endpoint for retrieving the current authenticated user's profile
- Password hashing using BCrypt with configurable work factor
- Default admin account seeded via EF Core data migration

#### Artist Management
- Artist registration flow with `Pending` status on signup
- Admin approval endpoint: `POST /api/v1/admin/artists/{artistId}/approve`
- Admin rejection endpoint with reason: `POST /api/v1/admin/artists/{artistId}/reject`
- Email notification dispatched to artist on approval or rejection
- Public artist profile endpoint: `GET /api/v1/artists/{artistId}`
- Artist profile self-update endpoint: `PUT /api/v1/artists/me`
- Artists with `Pending` or `Rejected` status cannot post listings or appear in public search

#### Artwork Listings
- Create artwork listing endpoint: `POST /api/v1/artworks`
- Update artwork listing endpoint: `PUT /api/v1/artworks/{artworkId}`
- Soft-delete artwork endpoint: `DELETE /api/v1/artworks/{artworkId}`
- Public artwork listing endpoint with pagination: `GET /api/v1/artworks`
- Artwork detail endpoint: `GET /api/v1/artworks/{artworkId}`
- Query filters for artwork listing: `medium`, `minPrice`, `maxPrice`, `search`, `sortBy`
- Artist's own artwork management endpoint: `GET /api/v1/artists/me/artworks`

#### Subscription & Tier System
- Two-tier subscription model: `Free` and `Premium`
- Free tier listing limit enforced at service layer (default: 5 artworks)
- `422 Unprocessable Entity` returned when free-tier limit is exceeded
- `Subscription` entity with `Tier`, `IsActive`, `StartDate`, and `EndDate` fields
- Tier configuration injectable via `SubscriptionSettings__FreeListingLimit` app setting

#### Order Request System
- Buyer order request endpoint: `POST /api/v1/orders`
- Order returns `artistProfileUrl` redirect upon creation, directing buyer to artist's profile
- Order detail endpoint accessible by buyer, artist, or admin: `GET /api/v1/orders/{orderId}`
- Artist order status update endpoint: `PATCH /api/v1/orders/{orderId}/status`
- Valid order status transitions enforced: `Pending → Accepted`, `Pending → Declined`, `Accepted → Completed`
- Artist's received orders listing with status filter: `GET /api/v1/artists/me/orders`
- Buyer's order history listing: `GET /api/v1/buyers/me/orders`
- Order denormalizes `ArtistId` for efficient notification and authorization lookup

#### Email Notifications
- HTML email dispatched to artist upon new order request
- HTML email dispatched to artist on admin approval
- HTML email dispatched to artist on admin rejection (includes rejection reason)
- Welcome email dispatched to all new users on registration
- Email templates stored as `.html` files with `{{token}}` placeholder substitution
- Configurable SMTP settings supporting SendGrid and standard SMTP servers
- Email dispatch is non-blocking (fire-and-forget) to prevent order API latency

#### AI Chatbot
- Chatbot message endpoint: `POST /api/v1/chatbot/message`
- Conversational art discovery powered by OpenAI GPT-4o
- Stateless design: full conversation history passed by client per request
- Chatbot returns natural language reply plus matched `ArtworkSummaryDto[]` from database
- Chatbot status health endpoint: `GET /api/v1/chatbot/status`
- System prompt engineered to guide buyers through style, medium, and budget discovery

#### Admin
- Admin paginated user list endpoint: `GET /api/v1/admin/users` with `role` filter
- Admin pending artist queue endpoint: `GET /api/v1/admin/artists/pending`
- Platform-level admin user seeded in initial migration

#### Infrastructure & Developer Experience
- Clean layered architecture: `API → Application → Domain → Infrastructure`
- Generic repository pattern with async CRUD methods
- AutoMapper profiles for all DTO mappings in `Application/Mappings/`
- FluentValidation validators for all request DTOs
- Global exception handler middleware with consistent error response format
- Swagger / OpenAPI 3.0 documentation (Development environment only)
- EF Core 8 with SQL Server and PostgreSQL compatibility
- Docker and Docker Compose support for containerized local development
- `.env.example` template for environment variable configuration
- xUnit + Moq unit test project scaffolding
- WebApplicationFactory integration test project scaffolding
- GitHub Actions CI workflow: build + test on every PR to `main`

### Security

- All sensitive configuration values externalized to environment variables
- `appsettings.Development.json` and `.env` files excluded via `.gitignore`
- JWT tokens carry minimum required claims (no PII beyond email)
- Artist resource operations (update artwork, view orders) validate ownership before executing
- Admin endpoints protected with both authentication and `Admin` role policy

---

## Links

[Unreleased]: https://github.com/your-org/artmarket/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/your-org/artmarket/releases/tag/v1.0.0