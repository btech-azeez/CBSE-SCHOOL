# CBSE School Management System

A full-featured **ASP.NET Core MVC** web application for managing a CBSE school — covering students, teachers, classes, subjects, exams, marks, fees, and notices.

---

## 🏫 Features

| Module | Description |
|---|---|
| **Students** | Add, edit, view and manage student records |
| **Teachers** | Manage teacher profiles and assignments |
| **Classes & Subjects** | Define school classes and map subjects to them |
| **Exam Marks** | Record and view student marks per exam/subject |
| **Fee Management** | Track fee structures and student payments |
| **Notices** | Post and manage school announcements |
| **Authentication** | Role-based login via ASP.NET Core Identity |

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 10)
- **Database:** SQL Server + Entity Framework Core 9
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Razor Views (HTML/CSS/JS)
- **ORM:** Entity Framework Core (Code-First)
- **Container:** Docker-ready (Windows Nano Server LTSC 2022)

---

## 📁 Project Structure

```
CBSESchool/
├── Controllers/           # MVC Controllers
├── Models/                # Domain Models
├── Data/                  # EF Core DbContext
├── Repositories/          # Data Access Layer
├── Services/              # Business Logic
├── Views/                 # Razor Views
├── wwwroot/               # Static Assets
├── Program.cs
├── appsettings.json
└── CBSESchool.csproj
```

---

## ⚙️ Prerequisites

- .NET 10 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022+ or VS Code

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/ShaikAhmed47/CBSE-School-.NET.git
cd CBSE-School-.NET
```

### 2. Configure the database connection

Edit `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CBSESchoolDb;Trusted_Connection=True;"
  }
}
```

### 3. Apply migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

---

## 🐳 Docker (Optional)

```bash
docker build -t cbseschool .
docker run -p 8081:8081 cbseschool
```

---

## 🧪 Tests

```bash
dotnet test
```

---

## 📄 License

This project is open source and available under the MIT License.

---

## 👤 Author

**Shaik Ahmed**
GitHub: https://github.com/ShaikAhmed47
