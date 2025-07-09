# 🧭 Microservice Architecture Guide

---

## 🚀 Technology Stack

### ⚙️ Core

- **.NET 6.0**

### 📚 Libraries & Frameworks

- `AutoMapper`
- `FluentValidation`
- `MediatR`
- `Entity Framework Core`
- `Carter`
- `Ocelot`
- `Serilog`
- `Pomelo MySQL Provider`
- `RabbitMQ`
- `Newtonsoft.Json`
- `MailKit`
- `SMTPEmail`
- `Masstransit.RabbitMQ`
- `MongoDb.Driver`
- `Grpc.AspNetCore`
- `Stateless`
- `Serilog.sinks.elasticsearch`
- `AspNetCore.HealthChecks.UI`
- `AspNetCore.HealthChecks.UI.InMemory.Storage`
- `AspNetCore.HealthChecks.UI.Client`
- `AspNetCore.HealthChecks.MySql`
- `Duende.IdentityServer`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`

### 🗄️ Databases

- `Redis`
- `PostgreSQL`
- `MySQL`
- `SQL Server`
- `MongoDB`
- `Elasticsearch`

### 🏗️ Architecture Patterns

- **Unit of Work**
- **Repository Pattern**
- **CQRS (Command Query Responsibility Segregation)**
- **Clean Architecture**

---

## 🛠️ Common Commands

### 📦 Database Migrations (EF Core)

#### 🔹 Basic

```bash
dotnet ef migrations add "<migration_name>"
dotnet ef database update
```

#### 🔹 Project-Specific

```bash
dotnet ef migrations add "<migration_name>" --project Ordering.Infrastructure --startup-project Ordering.API --output-dir Persistence/Migrations

dotnet ef database update --project Ordering.Infrastructure --startup-project Ordering.API
```

### 🐳 Docker Compose

```bash
cd src
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```

## 🌐 Service Access

| 🧩 Service | 🌍 URL                 | 👤 Username | 🔐 Password     |
| ---------- | ---------------------- | ----------- | --------------- |
| Portainer  | http://localhost:9000  | admin       | Nguyenkhai2611! |
| Kibana     | http://localhost:5601  | elastic     | admin           |
| RabbitMQ   | http://localhost:15672 | guest       | guest           |
