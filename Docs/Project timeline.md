# Project Timeline  
**Schedule:** 2 hrs/day · 6 days/week · ~6 months  

---

## 1. Foundation (Weeks 1–2)
**~24 hrs · Goal:** Project compiles and connects to DB  

- Restructure GitHub repo  
- Create solution with 3 projects (API / Core / Infra)  
- Install EF Core + PostgreSQL  
- Create all entities + AppDbContext  
- Run first migration  
- Configure DI in `Program.cs`  
- Add health check endpoint  

---

## 2. Authentication (Weeks 3–4)
**~24 hrs · Goal:** Register, login, JWT working  

- User registration + BCrypt hashing  
- JWT token generation  
- Refresh token flow  
- Role assignment  
- `[Authorize]` on test endpoints  
- Postman collection for auth  

---

## 3. Artist Flows (Weeks 5–7)
**~36 hrs · Goal:** Artist can apply, admin can approve  

- Artist registration / application  
- Admin approval/rejection endpoints  
- Email notification via SendGrid  
- Artist profile CRUD  
- Repository + service layers  
- FluentValidation on DTOs  

---

## 4. Artwork Management (Weeks 8–10)
**~36 hrs · Goal:** Full artwork CRUD with images  

- Artwork CRUD endpoints  
- Cloudinary image upload service  
- Listing type (display / for-sale)  
- Subscription tier limit enforcement  
- Tags + category filtering  
- Pagination on listing endpoints  

---

## 5. Orders + Subscriptions (Weeks 11–13)
**~36 hrs · Goal:** Buyer can order, artist gets notified, Stripe works  

- Order + custom request endpoints  
- Email to artist on new order  
- Stripe integration + webhook handler  
- Subscription tier upgrade flow  
- Artist dashboard endpoint  

---

## 6. Chatbot + Caching (Weeks 14–15)
**~24 hrs · Goal:** Chatbot returns artwork recommendations  

- ChatbotService with Anthropic API  
- ChatbotController  
- IMemoryCache on listing endpoints  
- Redis setup (optional)  
- Chatbot Postman tests  

---

## 7. Docker + CI/CD (Week 16)
**~12 hrs · Goal:** `docker-compose up` runs everything  

- Dockerfile (multi-stage build)  
- docker-compose with API + PostgreSQL + Redis  
- GitHub Actions pipeline (build → test → lint)  
- Environment variables via `.env` file  

---

## 8. Next.js Frontend (Weeks 17–22)
**~72 hrs · Goal:** Working UI connected to your API  

- Browse artworks page  
- Artist profile page  
- Artwork detail + order flow  
- Chatbot widget  
- Login/register forms  
- Artist dashboard  
- Responsive mobile layout  
- Deploy to Vercel  

---

## 9. Polish + Interview Prep (Weeks 23–24)
**~24 hrs · Goal:** Portfolio-ready  

- Unit tests for services  
- Swagger / OpenAPI docs  
- README with architecture diagram + demo GIF  
- Deploy API to Render or Railway  
- Deploy frontend to Vercel  
- Practice explaining design decisions  

---