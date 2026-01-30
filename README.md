# SSMS - Quản Lý Quy Trình An Toàn & Biểu Mẫu QHSE

Hệ thống quản lý quy trình an toàn và biểu mẫu QHSE cho doanh nghiệp.

## 🚀 Tech Stack

- **Backend**: .NET 8, ASP.NET Core Web API, Entity Framework Core
- **Frontend**: React 19, Vite, TypeScript
- **Database**: SQL Server 2022
- **Deployment**: Docker, Nginx

## 📦 Quick Start

### Development

```bash
# Backend
cd backend/src/SSMS.API
dotnet restore
dotnet run

# Frontend
cd frontend
npm install
npm run dev
```

### Docker Deployment

```bash
# Setup
cp .env.example .env
# Edit .env with your configuration

# Run
docker compose up -d
```

**Access**: http://localhost:3000

Xem [DOCKER-README.md](./DOCKER-README.md) để biết chi tiết deployment.

## 📁 Project Structure

```
├── backend/
│   ├── src/
│   │   ├── SSMS.API/          # Web API
│   │   ├── SSMS.Application/  # Business logic
│   │   ├── SSMS.Core/         # Domain models
│   │   └── SSMS.Infrastructure/ # Data access
│   └── tests/                 # Unit & integration tests
├── frontend/                  # React frontend
├── docs/                      # Documentation
└── docker-compose.yml         # Docker orchestration
```

## 🧪 Testing

```bash
# Backend tests
dotnet test backend/SSMS.sln

# Frontend tests
cd frontend && npm test
```

## 📖 Documentation

- [Docker Deployment Guide](./DOCKER-README.md)
- [Database Migration Guide](./docs/DATABASE_MIGRATION_GUIDE.md)
- [UI/UX Guidelines](./docs/design-system/UI-UX-RULES.md)

## 🔐 Security

- JWT authentication
- Role-based access control (RBAC)
- SQL injection protection via EF Core parameterized queries

## 📝 License

Private project - All rights reserved
