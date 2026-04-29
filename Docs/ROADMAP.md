# ROADMAP.md — ArtMarket Future Plans

> This document describes what's been built, what's coming next, and the long-term vision for the ArtMarket platform. It is a living document and will be updated as priorities evolve.

---

## Summary

| Phase | Theme | Status |
|-------|-------|--------|
| [Phase 1 — MVP](#phase-1--mvp) | Core marketplace functionality | In Progress |
| [Phase 2 — Enhancements](#phase-2--enhancements) | Discoverability, trust, and retention | Planned |
| [Phase 3 — Scale](#phase-3--scale) | Mobile, payments, analytics | Planned |

---

## Phase 1 — MVP

**Status: In Progress** 

The MVP establishes the full foundation of the marketplace: artist onboarding, artwork listings, buyer order requests, and the AI discovery chatbot.

### Features

- [ ] **User registration & login** — JWT authentication with role-based authorization (Admin, Artist, Buyer)
- [ ] **Artist approval workflow** — Artists register as `Pending` and are manually approved by an Admin
- [ ] **Artist profiles** — Public-facing artist pages with bio, profile image, and active listings
- [ ] **Artwork listings** — Artists can create, update, and soft-delete artwork listings with title, description, price, medium, and image
- [ ] **Tiered listing system** — Free tier capped at 5 listings; Premium tier is unlimited
- [ ] **Order request system** — Buyers submit order requests with a personal message; artist receives the request
- [ ] **Artist redirect** — After placing an order, buyers are redirected to the artist's profile to finalize the deal directly
- [ ] **Email notifications** — Automated emails to artists on new orders, approval, and rejection
- [ ] **AI chatbot** — Natural language art discovery powered by GPT-4o, returns matched artwork from the database
- [ ] **Public artwork browse** — Paginated, filterable listing of available artworks from approved artists
- [ ] **Admin tools** — Manage users and handle artist approval queue
- [ ] **Docker support** — Full containerized local development environment
- [ ] **Swagger docs** — Interactive API documentation in Development

---

## Phase 2 — Enhancements

**Status: Planned**

Phase 2 focuses on deepening trust between buyers and artists, improving artwork discoverability, and giving artists better tools to manage their presence.

### Planned Features

#### Trust & Social Proof
- [ ] **Buyer reviews on completed orders** — Buyers can leave a rating (1–5 stars) and written review after an order is marked `Completed`
- [ ] **Artist rating aggregation** — Artists display an average rating and review count on their public profile
- [ ] **Review moderation** — Admin can flag and remove abusive reviews
- [ ] **Featured artists** — Admin can mark certain artists as "Featured" to appear at the top of discovery pages

#### Search & Discovery
- [ ] **Tag system** — Artists can tag artwork with style keywords (e.g., `Abstract`, `Portrait`, `Landscape`) to improve searchability
- [ ] **Advanced artwork search** — Full-text search across title, description, medium, and tags
- [ ] **Faceted filtering** — Filter by style, color palette, size range, and artist tier
- [ ] **Recommended artworks** — "More from this artist" and "Similar artworks" sections on the artwork detail page
- [ ] **Trending artworks** — Surface artworks with the most order requests in the last 7 days

#### Buyer Experience
- [ ] **Saved/favorited artworks** — Buyers can save artworks to a personal wishlist
- [ ] **Order messaging thread** — Lightweight in-platform message thread per order for buyer-artist communication before finalizing
- [ ] **Order history export** — Buyers can export their order history as a CSV

#### Artist Tools
- [ ] **Listing analytics dashboard** — Artists can see views, order requests, and save counts per artwork
- [ ] **Subscription self-service** — Artists can upgrade to Premium and manage their subscription status (pre-payment gateway: manual activation by admin)
- [ ] **Artwork status toggle** — Artists can mark artwork as `Sold` (not just unavailable) to show buyers it's gone
- [ ] **Bulk artwork operations** — Mark multiple artworks available/unavailable at once

#### Developer & Platform
- [ ] **Postman collection** — Export full API as a Postman v2.1 collection
- [ ] **Webhook support** — Artists can configure a webhook URL to receive order events
- [ ] **Rate limiting** — Per-IP and per-user rate limiting on public and auth endpoints
- [ ] **Structured logging with Serilog** — Replace default ILogger with Serilog + Seq for searchable structured logs

---

## Phase 3 — Scale

**Status: Planned**

Phase 3 transforms ArtMarket from a functional marketplace into a scalable, monetized platform with a mobile presence and data-driven growth tools.

### Planned Features

#### Mobile Application
- [ ] **React Native mobile app** — iOS and Android app consuming the existing REST API
- [ ] **Push notifications** — Artists and buyers receive push notifications for order updates and messages
- [ ] **Camera-to-listing** — Artists can photograph and list artwork directly from their phone
- [ ] **Augmented reality preview** — Buyers can visualize artwork on their wall using device camera (AR kit integration)

#### Payments
- [ ] **Stripe integration** — Artist subscription payments (Free → Premium upgrade)
- [ ] **Escrow payment option** — Optional platform-mediated payment where the buyer pays through the platform and funds are released to the artist on order completion
- [ ] **Artist payout management** — Artists configure a Stripe Connect account for payouts
- [ ] **Transaction fee model** — Platform takes a configurable percentage of escrow transactions
- [ ] **Invoice generation** — Auto-generated PDF invoice for completed escrow orders

#### Analytics & Intelligence
- [ ] **Admin analytics dashboard** — Platform-wide metrics: GMV, active artists, order volume, chatbot engagement
- [ ] **Artist growth insights** — Week-over-week trends for profile views, listing performance, and revenue
- [ ] **Chatbot improvement pipeline** — Log anonymized chatbot queries to improve recommendation quality
- [ ] **A/B testing framework** — Test different homepage layouts, search result rankings, and recommendation algorithms

#### Infrastructure & Operations
- [ ] **CDN integration** — Artwork images served via a CDN (Cloudflare / AWS CloudFront) for global performance
- [ ] **Image processing pipeline** — Auto-resize and optimize uploaded artwork images; generate multiple resolution variants
- [ ] **Redis caching** — Cache public artwork listings and artist profiles for high-traffic scenarios
- [ ] **Horizontal scaling** — Kubernetes deployment manifests for cloud scaling
- [ ] **Multi-region deployment** — Primary region: Egypt/MENA; secondary: Europe
- [ ] **GDPR compliance tooling** — Data export and account deletion endpoints for EU buyers

#### Community & Growth
- [ ] **Artist blog / portfolio page** — Rich text editor for artists to publish articles or behind-the-scenes content
- [ ] **Collections** — Buyers can curate and share named collections of saved artworks
- [ ] **Referral program** — Artists earn extended Premium trial for referring other artists who get approved
- [ ] **Multi-language support (i18n)** — Arabic and English as primary languages; extensible to others

---

## Contributing to the Roadmap

Have a feature idea not listed here? Open a [Feature Request](https://github.com/your-org/artmarket/issues/new?template=feature_request.md) on GitHub. If a feature aligns with the platform's direction and has community interest, it will be added to a future phase.
