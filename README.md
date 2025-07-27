# 🏗️ Enterprise Microservice Architecture

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Usage](#usage)
- [Architecture Patterns](#architecture-patterns)
- [Database Operations](#database-operations)

- [Acknowledgments](#acknowledgments)

## Introduction

This project is a comprehensive **Enterprise Microservice Architecture** built with .NET 6.0, designed to demonstrate modern distributed system patterns and best practices. The solution implements a production-ready microservice ecosystem featuring event-driven architecture, CQRS patterns, and comprehensive observability.

**Why this project exists:**
- Showcase enterprise-level microservice implementation
- Demonstrate clean architecture principles in distributed systems
- Provide a reference architecture for scalable, maintainable applications
- Implement industry-standard patterns for fault tolerance and resilience

The architecture serves as a foundation for building complex, distributed applications that can scale horizontally and maintain high availability.

## Features

✅ **Event-Driven Architecture** - Asynchronous communication between services  
✅ **CQRS Implementation** - Separate read/write operations for optimal performance  
✅ **API Gateway** - Centralized routing and cross-cutting concerns  
✅ **Distributed Transactions** - SAGA pattern for complex business workflows  
✅ **Multi-Database Support** - Polyglot persistence with PostgreSQL, MySQL, MongoDB, Redis  
✅ **Identity Management** - OAuth 2.0/OpenID Connect with Duende IdentityServer  
✅ **Comprehensive Logging** - Structured logging with Elasticsearch and Kibana  
✅ **Health Monitoring** - Service health checks and dependency validation  
✅ **Background Processing** - Reliable job processing with Hangfire  
✅ **Resilience Patterns** - Circuit breakers, retries, and timeout handling  
✅ **Container Orchestration** - Docker containerization with docker-compose  
✅ **Message Queuing** - RabbitMQ with MassTransit for reliable messaging

## Technologies Used

### Core Framework
- **.NET 6.0** - Primary development platform

### Communication & Integration
- **Ocelot** - API Gateway for routing and load balancing
- **RabbitMQ** - Enterprise message broker for async communication
- **MassTransit** - Distributed application framework
- **gRPC** - High-performance RPC communication

### Data Access & ORM
- **Entity Framework Core** - Primary ORM with code-first approach
- **Dapper** - Lightweight ORM for performance-critical queries
- **AutoMapper** - Object-to-object mapping
- **Pomelo MySQL Provider** - MySQL Entity Framework provider

### Application Patterns & Libraries
- **MediatR** - In-process messaging for CQRS implementation
- **FluentValidation** - Fluent interface for validation rules
- **Polly** - Resilience and transient-fault-handling
- **Stateless** - State machine implementation

### Security & Authentication
- **Duende IdentityServer** - OAuth 2.0 & OpenID Connect framework

### Monitoring & Observability
- **Serilog** - Structured logging with multiple sinks
- **Elasticsearch** - Search and analytics engine
- **Kibana** - Data visualization and log analysis
- **AspNetCore.HealthChecks** - Service health monitoring

### Background Services
- **Hangfire** - Background job processing and scheduling

### Database Systems
- **PostgreSQL** - Primary relational database
- **MySQL** - Alternative relational database
- **SQL Server** - Microsoft relational database
- **MongoDB** - Document database
- **Redis** - In-memory cache and session store
- **Elasticsearch** - Full-text search engine

### Development Tools
- **Docker** - Containerization platform
- **Portainer** - Container management interface

## Installation

### Prerequisites
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Step 1: Clone Repository
```bash
git clone <repository-url>
cd <project-directory>
```

### Step 2: Setup HTTPS Certificates
**Windows:**
```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\microservice.pfx -p SecurePassword123
dotnet dev-certs https --trust
```

**macOS/Linux:**
```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/microservice.pfx -p SecurePassword123
dotnet dev-certs https --trust
```

### Step 3: Start Infrastructure Services
```bash
cd src
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```

### Step 4: Initialize Databases
Run the database migration commands from the [Database Operations](#database-operations) section.

## Usage

### Starting the Application
After installation, the services will be available at their respective endpoints:

**Local Environment (Development):**
- Services will be accessible on ports starting with **500x** (5001, 5002, 5003, etc.)
- Example: API Gateway at http://localhost:5000

**Development Environment:**
- Services will be accessible on ports starting with **600x** (6001, 6002, 6003, etc.)
- Example: Identity Server at http://localhost:6001

### Service Management
Use Docker and monitoring tools to manage and monitor the infrastructure services. Access these through your Docker environment or container orchestration platform.

### Development Workflow
1. **Add new services** following the established clean architecture structure
2. **Implement CQRS patterns** using MediatR for commands and queries
3. **Use event-driven communication** via RabbitMQ for service integration
4. **Apply database migrations** when making schema changes
5. **Monitor application health** through the health check endpoints

## Architecture Patterns

### Core Patterns
- **Clean Architecture** - Dependency inversion with clear separation of concerns
- **CQRS** - Command Query Responsibility Segregation for scalable operations
- **Repository Pattern** - Data access abstraction layer
- **Unit of Work** - Coordinated transaction management

### Distributed System Patterns
- **Event-Driven Architecture** - Asynchronous, loosely-coupled service communication
- **Event Sourcing** - Event-based state management and audit trail
- **Saga Pattern** - Distributed transaction coordination across services
- **API Gateway Pattern** - Single entry point with cross-cutting concerns
- **Circuit Breaker** - Fault tolerance and resilience patterns

## Database Operations

### Standard Entity Framework Migrations
```bash
# Create new migration
dotnet ef migrations add "MigrationName"

# Apply to database
dotnet ef database update
```

### Project-Specific Migrations
```bash
dotnet ef migrations add "MigrationName" \
  --project Ordering.Infrastructure \
  --startup-project Ordering.API \
  --output-dir Persistence/Migrations

dotnet ef database update \
  --project Ordering.Infrastructure \
  --startup-project Ordering.API
```

### Identity Server Database Setup
```bash
# Navigate to IdentityServer directory first
cd IdentityServer

# Update IdentityServer contexts
dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext

# Add custom identity migrations
dotnet ef migrations add "Init_Identity" \
  -c IdentityContext \
  -s IdentityServer/IdentityServer.csproj \
  -p ./Microservice.IDP.Infrastructure/Microservice.IDP.Infrastructure.csproj \
  -o Persistence/Migrations

# Update identity database
dotnet ef database update \
  -c IdentityContext \
  -s IdentityServer/IdentityServer.csproj \
  -p ./Microservice.IDP.Infrastructure/Microservice.IDP.Infrastructure.csproj
```

## Acknowledgments

This project was inspired by and built upon the knowledge and patterns from various sources in the microservices and distributed systems community.

**Special thanks to [Tedu Community](https://tedu.com.vn) for providing excellent learning resources, mentorship, and fostering a collaborative environment for developers in the .NET ecosystem.**

The architectural patterns and implementation approaches in this project draw from industry best practices and the collective wisdom of the developer community, helping to create a robust foundation for enterprise-level microservice development.