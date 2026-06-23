# Enterprise Clean Architecture Boilerplate

A production-ready ASP.NET Core starter template built using Clean Architecture principles and modern development practices. This project provides a solid foundation for building scalable, maintainable, secure, and testable enterprise applications.

## Features

* Clean Architecture
* ASP.NET Core Web API
* CQRS with MediatR
* Entity Framework Core
* SQL Server
* JWT Authentication
* Role-Based Authorization
* FluentValidation
* Global Exception Handling
* Serilog Logging
* Swagger / OpenAPI
* Docker Support
* GitHub Actions CI/CD
* Unit Testing
* Integration Testing

## Architecture

```text
src/
├── Enterprise.Api
├── Enterprise.Application
├── Enterprise.Domain
└── Enterprise.Infrastructure

tests/
├── Enterprise.UnitTests
└── Enterprise.IntegrationTests
```

### Layer Responsibilities

#### Domain

Contains core business entities, enums, constants, and domain rules.

#### Application

Contains business use cases, CQRS handlers, DTOs, validators, interfaces, and application logic.

#### Infrastructure

Contains database access, external services, authentication, logging, caching, and integrations.

#### API

Contains controllers, middleware, API configuration, and dependency injection setup.

## Technology Stack

* .NET 10.0
* ASP.NET Core
* Entity Framework Core
* MediatR
* FluentValidation
* SQL Server
* Serilog
* JWT Bearer Authentication
* Docker
* xUnit

## Getting Started

### Clone Repository

```bash
git clone https://github.com/sharif-azharul/enterprise-clean-architecture-boilerplate.git
```

### Navigate to Project

```bash
cd enterprise-clean-architecture-boilerplate
```

### Restore Dependencies

```bash
dotnet restore
```

### Apply Database Migrations

```bash
dotnet ef database update
```

### Run Application

```bash
dotnet run --project src/Enterprise.Api
```

### Open Swagger

```text
https://localhost:7247/swagger
```

## Development Principles

* Separation of Concerns
* Dependency Inversion
* SOLID Principles
* Testability First
* Security by Default
* Clean Code Practices
* Production-Ready Configuration

## Roadmap

### v1.0

* Clean Architecture
* CQRS
* JWT Authentication
* Validation
* Logging
* Docker
* CI/CD

### v2.0

* Permission-Based Authorization
* Audit Trail
* Redis Caching
* Background Jobs
* Multi-Tenant Support
* Outbox Pattern

### v3.0

* Azure Deployment Templates
* Event-Driven Architecture
* Dataverse / Dynamics 365 Integration
* SaaS Accelerator Features

## Contributing

Contributions, suggestions, and improvements are welcome. Feel free to open an issue or submit a pull request.

## License

This project is licensed under the MIT License.

---

Built for developers who want to start enterprise-grade ASP.NET Core projects with a clean, scalable, and maintainable foundation.
