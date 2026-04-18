# 🎨 ArtMarket

> A marketplace where local artists showcase and sell their work — buyers discover, inquire, and connect directly with the artist.

---

## What It Does

ArtMarket is a REST API that connects three types of users:

- **Admin** — approves artists before they can list work
- **Artist** — creates a profile, lists artwork, receives order requests
- **Buyer** — browses artwork, sends order requests, finalizes deals directly with the artist

The platform handles **discovery and ordering only**. Shipping, payment, and logistics are agreed upon directly between the buyer and the artist.

---

## Current Prototype (v1.0)

This is what's built and working right now:

### Auth
- Register as a Buyer or Artist
- Login and receive a JWT token
- Role-based access: Admin / Artist / Buyer

### Artist Flow
1. Artist registers → status is `Pending`
2. Admin reviews and approves (or rejects) the artist
3. Approved artist can create and manage artwork listings
4. Free tier: up to **5 listings**. Premium tier: **unlimited**

### Buyer Flow
1. Buyer browses public artwork listings
2. Buyer sends an order request for **one artwork at a time**
3. Artist receives an **email notification**
4. Buyer is redirected to the **artist's profile** to finalize the deal directly

### Chatbot
- Buyer describes what they're looking for in plain language
- AI assistant (GPT-4o) suggests matching artworks from the database

---

## Known Limitations in v1.0

| Limitation | Reason |
|------------|--------|
| One artwork per order | No cart system yet — each order is a direct inquiry to one artist |
| No in-platform payment | Platform does not handle money — deal is finalized off-platform |
| No reviews or ratings | Planned for v2.0 |
| Manual subscription upgrade | No payment gateway yet — admin activates Premium manually |
| No real-time messaging | Buyer and artist communicate via email after the order request |

---

## Future Extensions

### v2.0 — Trust & Discovery
- [ ] Buyer reviews and artist ratings after completed orders
- [ ] Tag system on artworks (style, medium, color palette)
- [ ] Advanced search and filtering
- [ ] Buyer wishlist / saved artworks
- [ ] Artist listing analytics (views, saves, order counts)

### v3.0 — Cart & Payments
- [ ] **Multi-item orders** — buyer can add multiple artworks from one artist into a single order request
- [ ] Stripe subscription payments (Free → Premium upgrade)
- [ ] Optional escrow payments through the platform
- [ ] Auto-generated invoice PDF on order completion

> **Why only one artist per cart?**
> Even in v3.0, a single order will be scoped to one artist at a time. Since there is no unified logistics — each artist ships independently — combining artworks from different artists into one order does not make practical sense. A buyer wanting pieces from three artists would send three separate orders.

### v4.0 — Scale
- [ ] Mobile app (iOS & Android)
- [ ] Push notifications
- [ ] CDN for artwork image delivery
- [ ] Multi-language support (Arabic / English)
- [ ] Admin analytics dashboard

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 8 Web API |
| Database | SQL Server + EF Core 8 |
| Auth | ASP.NET Identity + JWT Bearer |
| Email | SendGrid / SMTP |
| Chatbot | OpenAI GPT-4o |
| Docs | Swagger / OpenAPI |
| Containers | Docker + Docker Compose |

---

## Quick Start

```bash
git clone https://github.com/your-org/artmarket.git
cd artmarket
dotnet restore
cp src/ArtMarket.API/appsettings.Example.json src/ArtMarket.API/appsettings.Development.json
# fill in your DB connection string, JWT secret, and API keys
dotnet ef database update --project src/ArtMarket.Infrastructure --startup-project src/ArtMarket.API
dotnet run --project src/ArtMarket.API
```

Swagger UI: `https://localhost:7001/swagger`  
Default admin: `admin@artmarket.io` / `Admin@123!`

For full setup instructions see [SETUP.md](SETUP.md).

---

## Docs

| Document | Description |
|----------|-------------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | System design, layers, ERD, auth flow |
| [API_DOCS.md](API_DOCS.md) | Full endpoint reference |
| [SETUP.md](SETUP.md) | Local dev, Docker, Cursor AI |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Branching, commits, PR checklist |
| [CHANGELOG.md](CHANGELOG.md) | Version history |
| [ROADMAP.md](ROADMAP.md) | Full future plans |

---

## License

MIT — see [LICENSE](LICENSE) 