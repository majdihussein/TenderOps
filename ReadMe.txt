# TenderOps 🧾

**TenderOps** is a full-stack web application for managing tenders, partners, and requests using ASP.NET Core. It follows Clean Architecture principles and provides a powerful admin dashboard and smooth user experience.

---

## 💡 Project Overview

TenderOps enables organizations to manage their tenders efficiently, including:
- Creating and updating tenders
- Assigning tenders to selected partners
- Handling requests from partners
- Rating products (available only after purchase)
- Role-based access for Admins and Partners

---

## 🚀 Key Features

- Clean Architecture with layered separation
- Authentication using Cookies with Refresh Token support
- Unit Testing using xUnit and Moq
- Dynamic homepage with brand-based filtering
- Custom-designed shopping cart with creative CSS
- Rating system that activates post-purchase
- Real-time partner dashboard for tender access

---



## 🛠️ Technologies Used

### Backend
- **ASP.NET Core 9.0**
- **C#** (Async/Await, LINQ, OOP)
- **Entity Framework Core** + Code First Migrations
- **AutoMapper**
- **IHttpClientFactory** + Token Refresh Logic
- **Repository & Service Pattern**

### Frontend
- **Razor Pages + Bootstrap 5**
- Custom CSS (for Navbar, Cart, Buttons)
- **Font Awesome** / **Bootstrap Icons**

### Authentication & Authorization
- **Cookie-based Authentication**
- **Access + Refresh Tokens**
- **Role-Based Access Control**

### Testing
- **xUnit**
- **Moq**
- **FakeHttpMessageHandler** (for mocking HttpClient)

### Dev Tools
- **Git** & **GitHub** (Version Control, Branching, PRs, Code Reviews)
- **Visual Studio 2022**

---

## 🧪 Unit Testing

Unit tests are implemented for:
- Service Layer (business logic)
- Controller Layer (API and MVC actions)

Mocking dependencies like HttpClient and services using Moq, with clear arrange-act-assert structure.

---

## ▶️ Getting Started

1. **Clone the repository:**

```bash
git clone https://github.com/majdihussein
