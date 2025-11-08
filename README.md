# 🔒 SecureChat API

**SecureChat** is a secure real-time chat backend built with **ASP.NET Core 8**, designed and developed by **Gayatri Yadkikar**.  
The project implements **JWT authentication**, **role-based authorization**, and **SignalR integration** for real-time communication.

---

## 🚀 Key Highlights

- 🔐 Implemented full **JWT Authentication and Authorization** from scratch.  
- 🧩 Debugged and resolved **401 Unauthorized** issues by properly configuring token validation and middleware order.  
- 🗃️ Integrated **Entity Framework Core** with **SQLite** for persistent message and user data storage.  
- 💬 Added **SignalR hub** for real-time messaging support.  
- 🧪 Integrated **Swagger** with Bearer Token Authentication for testing APIs securely.  
- ⚙️ Designed for scalability, modularity, and clean architecture.

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
