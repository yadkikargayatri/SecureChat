# 🔒 SecureChat API

**SecureChat** is a secure real-time chat backend built with **ASP.NET Core 8**, designed and developed by **Gayatri Yadkikar**.  
The project implements **JWT authentication**, **role-based authorization**, and **SignalR integration** for real-time communication.

---

## 🚀 Key Highlights
## Features
- JWT authentication (register/login)
- SignalR real-time messaging
- Message persistence with EF Core (SQLite)
- Message history API
- Typing indicators & presence
- Minimal web UI demo

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-------------|
| Backend | ASP.NET Core 8, C# |
| Database | SQLite (Entity Framework Core) |
| Authentication | JWT (JSON Web Token) |
| Real-Time Communication | SignalR |
| Documentation | Swagger |
| Security | BCrypt password hashing |

---

## ⚙️ Setup Instructions

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/<your-username>/SecureChat.git
cd SecureChat

## Getting started
1. Update `appsettings.json` connection string and JWT secret.
2. Apply migrations:

dotnet ef migrations add Init
dotnet ef database update

3. Run the API:


dotnet run --project SecureChat

4. Run CLI client (optional):


dotnet run --project SignalRTestClient

5. Open `http://localhost:5234/chat.html` for web demo (enter JWT token).

## Architecture
- ASP.NET Core WebAPI + SignalR
- EF Core for persistence
- Simple console client for demo