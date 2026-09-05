# 🚀 DOL Platform — Phase 1: Identity Microservice

> **Enterprise-grade Identity & Authentication Microservice** built with .NET 10, Clean Architecture, CQRS Pattern, and Domain-Driven Design (DDD).

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Technology Stack](#-technology-stack)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Domain Model](#-domain-model)
- [API Endpoints](#-api-endpoints)
- [Authentication & Security](#-authentication--security)
- [Getting Started](#-getting-started)
- [Docker Deployment](#-docker-deployment)
- [Testing](#-testing)
- [Configuration](#-configuration)
- [Phase 1 Completion Summary](#-phase-1-completion-summary)

---

## 🎯 Overview

**DOL (Deal Online)** is a multi-service e-commerce platform designed with a microservices architecture. **Phase 1** focuses entirely on the **Identity Microservice** — the foundational service that handles all user authentication, authorization, role management, and token lifecycle.

### Key Features Delivered in Phase 1

| Feature | Description |
|---------|-------------|
| **User Registration** | Self-service signup with role assignment (Buyer/Dealer) |
| **Login with JWT** | Email/password authentication returning JWT access + refresh tokens |
| **Refresh Token Rotation** | Secure token refresh with old token revocation |
| **Change Password** | Authenticated users can update their password |
| **Forgot Password** | Email-based password reset token generation |
| **Reset Password** | Token-validated password reset flow |
| **Role Management** | Admin-only role assignment (Admin, Buyer, Dealer) |
| **User Profile** | Authenticated users can view their profile |
| **User Listing** | Admin-only paginated user listing |
| **Account Lockout** | Auto-lockout after 5 failed login attempts (15 min) |
| **API Gateway** | YARP-based reverse proxy routing to microservices |
| **Health Checks** | `/health` endpoints on both Gateway and Identity API |
| **Swagger UI** | Interactive API documentation with JWT auth support |
| **Docker Support** | Full `docker-compose` setup with PostgreSQL |
| **Unit Tests** | Domain entity and validator tests with xUnit |

---

## 🛠 Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Runtime** | .NET | `10.0` |
| **Language** | C# | `13` |
| **Database** | PostgreSQL | `16 (Alpine)` |
| **ORM** | Entity Framework Core | `9.0.2` |
| **CQRS / Mediator** | MediatR | `12.4.1` |
| **Validation** | FluentValidation | `11.11.0` |
| **Mapping** | Mapster | `7.4.0` |
| **Authentication** | JWT Bearer Tokens | `9.0.2` |
| **Password Hashing** | BCrypt.Net-Next | `4.0.3` |
| **API Gateway** | YARP Reverse Proxy | `2.2.0` |
| **Logging** | Serilog | `9.0.0` |
| **API Docs** | Swashbuckle (Swagger) | `7.3.1` |
| **Testing** | xUnit + Moq + FluentAssertions | Latest |
| **Containerization** | Docker + Docker Compose | `v3.8` |

---

## 🏗 Architecture

### Clean Architecture + CQRS

The project follows **Clean Architecture** with strict dependency inversion. Inner layers have zero knowledge of outer layers.

```
┌────────────────────────────────────────────────────┐
│                    API Layer                        │
│         (Controllers, Program.cs, Swagger)          │
├────────────────────────────────────────────────────┤
│              Infrastructure Layer                   │
│    (EF Core, PostgreSQL, JWT, BCrypt, Email)        │
├────────────────────────────────────────────────────┤
│              Application Layer                      │
│   (Commands, Queries, Handlers, Validators, DTOs)   │
├────────────────────────────────────────────────────┤
│                Domain Layer                         │
│       (Entities, Enums, Events, Value Objects)       │
├────────────────────────────────────────────────────┤
│               SharedKernel                          │
│     (BaseEntity, Result Pattern, Domain Events)      │
└────────────────────────────────────────────────────┘
```

### System Architecture Diagram

```
                         ┌──────────────┐
                         │   Clients    │
                         │ (Web / App)  │
                         └──────┬───────┘
                                │
                         ┌──────▼───────┐
                         │  DOL.Gateway │
                         │  (YARP Proxy)│
                         │  Port: 5000  │
                         └──────┬───────┘
                                │
                    ┌───────────▼───────────┐
                    │  DOL.Identity.API     │
                    │  Port: 5065           │
                    │  /api/auth/*          │
                    │  /api/user/*          │
                    └───────────┬───────────┘
                                │
                    ┌───────────▼───────────┐
                    │   PostgreSQL 16       │
                    │   Database: dol       │
                    │   Port: 5432          │
                    └───────────────────────┘
```

### CQRS Flow

```
HTTP Request
    │
    ▼
Controller ──▶ MediatR.Send(Command/Query)
                        │
            ┌───────────┴───────────┐
            ▼                       ▼
    CommandHandler           QueryHandler
            │                       │
            ▼                       ▼
   Domain Entities            EF Core DbSet
   (Business Logic)          (Read Operations)
            │                       │
            ▼                       │
    DbContext.SaveChanges()         │
            │                       │
            ▼                       ▼
         Result<T> ◀────────────────┘
            │
            ▼
    Controller.HandleResult() ──▶ HTTP Response
```

---

## 📁 Project Structure

```
DOL/
├── DOL.slnx                                    # Solution file (.NET 10 slnx format)
├── Directory.Build.props                        # Global build properties (net10.0, nullable, etc.)
├── Directory.Packages.props                     # Central Package Management (all NuGet versions)
├── docker-compose.yml                           # Docker orchestration (PostgreSQL + API + Gateway)
├── .gitignore                                   # Git exclusions
├── README.md                                    # ◄ You are here
│
├── src/
│   ├── DOL.SharedKernel/                        # ── Shared Kernel ──
│   │   ├── BaseEntity.cs                        #    Base entity with Id (Guid) + Domain Events
│   │   ├── AuditableEntity.cs                   #    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
│   │   ├── Result.cs                            #    Result<T> pattern (Success/Failure)
│   │   ├── IDomainEvent.cs                      #    Domain Event interface
│   │   └── IUnitOfWork.cs                       #    Unit of Work interface
│   │
│   ├── DOL.Gateway/                             # ── API Gateway ──
│   │   ├── Program.cs                           #    YARP reverse proxy setup
│   │   ├── Dockerfile                           #    Multi-stage Docker build
│   │   ├── appsettings.json                     #    Route config: /api/identity/** → Identity API
│   │   └── DOL.Gateway.csproj
│   │
│   └── Services/
│       └── Identity/
│           ├── DOL.Identity.Domain/             # ── Domain Layer ──
│           │   ├── Entities/
│           │   │   ├── ApplicationUser.cs       #    Rich domain model with behaviors
│           │   │   ├── ApplicationRole.cs       #    Admin, Buyer, Dealer roles
│           │   │   ├── RefreshToken.cs          #    JWT refresh token entity
│           │   │   └── UserRole.cs              #    Many-to-many join entity
│           │   ├── Enums/
│           │   │   └── UserStatus.cs            #    Pending, Active, Suspended
│           │   └── Events/
│           │       └── UserRegisteredEvent.cs   #    Domain event on user creation
│           │
│           ├── DOL.Identity.Application/        # ── Application Layer (CQRS) ──
│           │   ├── Commands/
│           │   │   ├── Register/                #    RegisterCommand + Handler + Validator
│           │   │   ├── Login/                   #    LoginCommand + Handler + Validator
│           │   │   ├── RefreshToken/            #    RefreshTokenCommand + Handler
│           │   │   ├── ChangePassword/          #    ChangePasswordCommand + Handler + Validator
│           │   │   ├── ForgotPassword/          #    ForgotPasswordCommand + Handler + Validator
│           │   │   ├── ResetPassword/           #    ResetPasswordCommand + Handler + Validator
│           │   │   └── AssignRole/              #    AssignRoleCommand + Handler
│           │   ├── Queries/
│           │   │   ├── GetUserProfile/          #    GetUserProfileQuery + Handler
│           │   │   └── GetAllUsers/             #    GetAllUsersQuery + Handler (paginated)
│           │   ├── DTOs/
│           │   │   └── AuthDtos.cs              #    UserDto, TokenResponseDto, AuthResultDto
│           │   ├── Behaviors/
│           │   │   └── ValidationBehavior.cs    #    MediatR validation pipeline behavior
│           │   ├── Interfaces/
│           │   │   ├── IIdentityDbContext.cs     #    DbContext abstraction
│           │   │   ├── IJwtTokenService.cs       #    JWT token generation/validation
│           │   │   ├── IPasswordHasher.cs        #    Password hashing abstraction
│           │   │   └── IEmailService.cs          #    Email service abstraction
│           │   └── DependencyInjection.cs        #    MediatR + FluentValidation registration
│           │
│           ├── DOL.Identity.Infrastructure/     # ── Infrastructure Layer ──
│           │   ├── Persistence/
│           │   │   ├── IdentityDbContext.cs      #    EF Core DbContext (PostgreSQL)
│           │   │   └── Configurations/
│           │   │       ├── ApplicationUserConfiguration.cs
│           │   │       ├── ApplicationRoleConfiguration.cs  # Seeded roles
│           │   │       ├── RefreshTokenConfiguration.cs
│           │   │       └── UserRoleConfiguration.cs         # Composite PK
│           │   ├── Migrations/
│           │   │   ├── InitialIdentityMigration.cs
│           │   │   └── AddPasswordResetTokenFields.cs
│           │   ├── Services/
│           │   │   ├── JwtTokenService.cs        #    HMAC-SHA256 JWT generation
│           │   │   ├── PasswordHasher.cs         #    BCrypt hashing
│           │   │   └── ConsoleEmailService.cs    #    Dev console email logger
│           │   └── DependencyInjection.cs        #    EF Core + JWT Auth + services registration
│           │
│           └── DOL.Identity.API/                # ── API Layer ──
│               ├── Controllers/
│               │   ├── ApiControllerBase.cs      #    Base controller with MediatR + Result handling
│               │   ├── AuthController.cs         #    Authentication endpoints
│               │   └── UserController.cs         #    User management endpoints
│               ├── Program.cs                    #    App bootstrap, middleware pipeline
│               ├── Dockerfile                    #    Multi-stage Docker build
│               ├── appsettings.json              #    Connection string, JWT config
│               └── DOL.Identity.API.csproj
│
└── tests/
    └── DOL.Identity.UnitTests/                  # ── Unit Tests ──
        ├── Domain/
        │   └── ApplicationUserTests.cs          #    Domain entity behavior tests
        ├── Commands/
        │   └── RegisterCommandValidatorTests.cs #    FluentValidation rule tests
        └── DOL.Identity.UnitTests.csproj
```

---

## 🧠 Domain Model

### Entity Relationship Diagram

```
┌──────────────────────────────┐       ┌──────────────────────────┐
│       ApplicationUser        │       │     ApplicationRole      │
├──────────────────────────────┤       ├──────────────────────────┤
│ Id            : Guid (PK)   │       │ Id          : Guid (PK)  │
│ FirstName     : string       │       │ Name        : string     │
│ LastName      : string       │       │ Description : string     │
│ Email         : string (UQ)  │       │ CreatedAt   : DateTime   │
│ PhoneNumber   : string       │       │                          │
│ PasswordHash  : string       │       │ Static IDs:              │
│ Status        : UserStatus   │       │  Admin  = c0a80101-..01  │
│ EmailConfirmed: bool         │       │  Buyer  = c0a80101-..02  │
│ AccessFailedCount : int      │       │  Dealer = c0a80101-..03  │
│ LockoutEnd    : DateTime?    │       └─────────┬────────────────┘
│ PasswordResetToken : string? │                  │
│ PasswordResetTokenExpiresAt  │                  │
│ CreatedAt     : DateTime     │       ┌──────────▼───────────────┐
│ UpdatedAt     : DateTime?    │       │       UserRole           │
│ CreatedBy     : string?      │       ├──────────────────────────┤
│ UpdatedBy     : string?      │◄─────►│ UserId : Guid (FK, CPK) │
└──────────────┬───────────────┘       │ RoleId : Guid (FK, CPK) │
               │                       └──────────────────────────┘
               │
    ┌──────────▼───────────────┐
    │      RefreshToken        │
    ├──────────────────────────┤
    │ Id              : Guid   │
    │ UserId          : Guid   │
    │ Token           : string │
    │ ExpiresAt       : DateTime│
    │ CreatedAt       : DateTime│
    │ CreatedByIp     : string │
    │ RevokedAt       : DateTime?│
    │ ReplacedByToken : string?│
    │                          │
    │ Computed:                │
    │  IsExpired : bool        │
    │  IsRevoked : bool        │
    │  IsActive  : bool        │
    └──────────────────────────┘
```

### Domain Behaviors (Rich Domain Model)

`ApplicationUser` is not an anemic model — it encapsulates business logic:

| Method | Business Rule |
|--------|--------------|
| `AddRole(roleId)` | Prevents duplicate role assignment |
| `RemoveRole(roleId)` | Safely removes if exists |
| `AddRefreshToken(...)` | Issues new refresh token |
| `UpdatePassword(hash)` | Updates password + audit timestamp |
| `ConfirmEmail()` | Marks email as confirmed |
| `UpdateProfile(...)` | Updates name + phone |
| `RecordFailedLogin()` | Increments counter, locks at 5 failures for 15 min |
| `ResetFailedLogin()` | Clears counter + lockout |
| `IsLockedOut` | Computed property checking lockout window |
| `SetPasswordResetToken(...)` | Sets token with configurable validity |
| `ValidatePasswordResetToken(...)` | Validates token + expiry |
| `ClearPasswordResetToken()` | Clears after successful reset |

### Domain Events

| Event | Trigger |
|-------|---------|
| `UserRegisteredEvent` | Raised when a new user is created via constructor |

---

## 🔌 API Endpoints

### Auth Controller — `api/auth`

| Method | Endpoint | Auth | Description | Request Body |
|--------|----------|------|-------------|-------------|
| `POST` | `/api/auth/register` | 🔓 Public | Register new user | `{ firstName, lastName, email, phoneNumber, password, role? }` |
| `POST` | `/api/auth/login` | 🔓 Public | Login with credentials | `{ email, password }` |
| `POST` | `/api/auth/refresh-token` | 🔓 Public | Refresh JWT tokens | `{ accessToken, refreshToken }` |
| `POST` | `/api/auth/change-password` | 🔒 Authenticated | Change user password | `{ currentPassword, newPassword }` |
| `POST` | `/api/auth/forgot-password` | 🔓 Public | Request password reset | `{ email }` |
| `POST` | `/api/auth/reset-password` | 🔓 Public | Reset password with token | `{ email, resetToken, newPassword }` |

### User Controller — `api/user`

| Method | Endpoint | Auth | Description | Request Body |
|--------|----------|------|-------------|-------------|
| `GET` | `/api/user/profile` | 🔒 Authenticated | Get current user profile | — |
| `GET` | `/api/user` | 🔒 Admin Only | Get all users (paginated) | Query: `?pageNumber=1&pageSize=10` |
| `POST` | `/api/user/{id}/assign-role` | 🔒 Admin Only | Assign role to user | `"RoleName"` (string body) |

### Response Format

**Success Response:**
```json
{
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "Ravi",
    "lastName": "Raj",
    "email": "ravi@example.com",
    "phoneNumber": "+919876543210",
    "status": "Active",
    "emailConfirmed": false,
    "roles": ["Buyer"],
    "createdAt": "2026-09-03T06:30:00Z"
  },
  "tokens": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJl...",
    "expiresAt": "2026-09-03T07:30:00Z"
  }
}
```

**Error Response:**
```json
{
  "errors": [
    "Email is already registered.",
    "Password must be at least 6 characters."
  ]
}
```

---

## 🔐 Authentication & Security

### JWT Token Configuration

| Parameter | Value | Description |
|-----------|-------|-------------|
| Algorithm | `HMAC-SHA256` | Symmetric key signing |
| Access Token Expiry | `60 minutes` | Configurable via `Jwt:ExpiryMinutes` |
| Refresh Token | `Base64 (64 bytes)` | Cryptographically random |
| Issuer | `DOL.Identity` | Configurable via `Jwt:Issuer` |
| Audience | `DOL.Platform` | Configurable via `Jwt:Audience` |

### JWT Claims

| Claim | Description |
|-------|-------------|
| `sub` | User ID (GUID) |
| `email` | User's email address |
| `given_name` | First name |
| `family_name` | Last name |
| `jti` | Unique token identifier |
| `role` | User's assigned roles (multiple) |

### Security Features

| Feature | Implementation |
|---------|---------------|
| **Password Hashing** | BCrypt with auto-generated salt |
| **Account Lockout** | 5 failed attempts → 15 min lockout |
| **Refresh Token Rotation** | Old token revoked on refresh, replaced by new token |
| **Token Validation** | Full validation (issuer, audience, lifetime, signing key) |
| **Result Pattern** | No exceptions for business logic — `Result<T>` ensures explicit error handling |
| **CORS** | Configurable CORS policy (currently `AllowAll` for development) |
| **HTTPS Redirection** | Enabled in middleware pipeline |

### Role-Based Authorization

| Role | Permissions |
|------|------------|
| **Admin** | Full access — manage users, assign roles, list all users |
| **Buyer** | View own profile, change password |
| **Dealer** | View own profile, change password |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL)
- [PostgreSQL 16](https://www.postgresql.org/) (or use Docker)

### Option 1: Run with Docker (Recommended)

```bash
# Clone the repository
git clone <repo-url>
cd DOL

# Start everything (PostgreSQL + Identity API + Gateway)
docker-compose up --build

# Services will be available at:
# Gateway:      http://localhost:5000
# Identity API: http://localhost:5065
# PostgreSQL:   localhost:5432
```

### Option 2: Run Locally

```bash
# 1. Start PostgreSQL (via Docker)
docker-compose up postgres -d

# 2. Restore NuGet packages
dotnet restore

# 3. Build the solution
dotnet build

# 4. Run the Identity API
dotnet run --project src/Services/Identity/DOL.Identity.API

# 5. (Optional) Run the Gateway in another terminal
dotnet run --project src/DOL.Gateway
```

### Option 3: Run with EF Core Migrations

```bash
# Apply migrations manually
dotnet ef database update \
  --project src/Services/Identity/DOL.Identity.Infrastructure \
  --startup-project src/Services/Identity/DOL.Identity.API
```

> **Note:** Migrations are auto-applied on application startup if PostgreSQL is running.

---

## 🐳 Docker Deployment

### Services

| Service | Image | Container | Port |
|---------|-------|-----------|------|
| **PostgreSQL** | `postgres:16-alpine` | `dol-postgres` | `5432` |
| **Identity API** | Custom (multi-stage build) | `dol-identity-api` | `5065 → 80` |
| **API Gateway** | Custom (multi-stage build) | `dol-gateway` | `5000 → 80` |

### Docker Compose Environment Variables

```yaml
# PostgreSQL
POSTGRES_DB: dol
POSTGRES_USER: postgres
POSTGRES_PASSWORD: postgrespassword

# Identity API
ASPNETCORE_ENVIRONMENT: Development
ConnectionStrings__DefaultConnection: Host=postgres;Database=dol;...
Jwt__SecretKey: SUPER_SECRET_KEY_FOR_DOL_PLATFORM_IDENTITY_SERVICE_NET10!
Jwt__Issuer: DOL.Identity
Jwt__Audience: DOL.Platform

# Gateway
ReverseProxy__Clusters__identity-cluster__Destinations__identity-service__Address: http://identity-api:80/
```

### Health Checks

```bash
# Identity API
curl http://localhost:5065/health

# Gateway
curl http://localhost:5000/health
```

---

## 🧪 Testing

### Test Project Structure

```
tests/DOL.Identity.UnitTests/
├── Domain/
│   └── ApplicationUserTests.cs         # Entity behavior tests
├── Commands/
│   └── RegisterCommandValidatorTests.cs # Validation rule tests
└── DOL.Identity.UnitTests.csproj
```

### Test Frameworks

| Package | Purpose |
|---------|---------|
| **xUnit** | Test framework |
| **Moq** | Mocking dependencies |
| **FluentAssertions** | Readable assertion syntax |
| **Microsoft.NET.Test.Sdk** | Test runner |

### Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~ApplicationUserTests"
```

---

## ⚙️ Configuration

### appsettings.json (Identity API)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=dol;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "SecretKey": "SUPER_SECRET_KEY_FOR_DOL_PLATFORM_IDENTITY_SERVICE_NET10!",
    "Issuer": "DOL.Identity",
    "Audience": "DOL.Platform",
    "ExpiryMinutes": "60"
  }
}
```

### Central Package Management

All NuGet package versions are centrally managed in `Directory.Packages.props` at the solution root. This ensures consistent versions across all projects.

### Build Properties

All projects share common settings via `Directory.Build.props`:
- **Target Framework:** `net10.0`
- **Nullable:** `enable`
- **Implicit Usings:** `enable`

---

## ✅ Phase 1 Completion Summary

### Layers Delivered

| Layer | Project | Status |
|-------|---------|--------|
| **Shared Kernel** | `DOL.SharedKernel` | ✅ Complete |
| **Domain** | `DOL.Identity.Domain` | ✅ Complete |
| **Application** | `DOL.Identity.Application` | ✅ Complete |
| **Infrastructure** | `DOL.Identity.Infrastructure` | ✅ Complete |
| **API** | `DOL.Identity.API` | ✅ Complete |
| **Gateway** | `DOL.Gateway` | ✅ Complete |
| **Tests** | `DOL.Identity.UnitTests` | ✅ Complete |
| **Docker** | `docker-compose.yml` | ✅ Complete |

### Features Summary

| Category | Items | Count |
|----------|-------|-------|
| **Domain Entities** | ApplicationUser, ApplicationRole, RefreshToken, UserRole | 4 |
| **Commands** | Register, Login, RefreshToken, ChangePassword, ForgotPassword, ResetPassword, AssignRole | 7 |
| **Queries** | GetUserProfile, GetAllUsers (paginated) | 2 |
| **Validators** | Register, Login, ChangePassword, ForgotPassword, ResetPassword | 5 |
| **Interfaces** | IIdentityDbContext, IJwtTokenService, IPasswordHasher, IEmailService | 4 |
| **Infrastructure Services** | JwtTokenService, PasswordHasher, ConsoleEmailService | 3 |
| **EF Configurations** | User, Role, RefreshToken, UserRole | 4 |
| **DB Migrations** | InitialIdentityMigration, AddPasswordResetTokenFields | 2 |
| **Controllers** | AuthController (6 endpoints), UserController (3 endpoints) | 9 endpoints |
| **Unit Tests** | ApplicationUserTests, RegisterCommandValidatorTests | 2 test classes |

### Design Patterns Used

| Pattern | Where Used |
|---------|-----------|
| **Clean Architecture** | Solution-wide layered dependency |
| **CQRS** | Commands (write) / Queries (read) separation |
| **MediatR** | In-process messaging and handler dispatch |
| **Result Pattern** | `Result<T>` for explicit error handling (no exceptions) |
| **Repository Pattern** | EF Core DbContext as repository |
| **Domain Events** | `UserRegisteredEvent` on entity creation |
| **Rich Domain Model** | Business logic inside entities (not anemic) |
| **Validation Pipeline** | `ValidationBehavior<TRequest, TResponse>` via MediatR |
| **Dependency Injection** | Extension methods per layer (`AddApplicationServices`, `AddInfrastructureServices`) |
| **Central Package Management** | `Directory.Packages.props` for version consistency |

---

## 📌 What's Next — Phase 2

Phase 2 will expand the platform with additional microservices. Potential services:

- 🛒 **Product/Catalog Service** — Product listings, categories, search
- 📦 **Order Service** — Order placement, status tracking
- 💳 **Payment Service** — Payment processing integration
- 📧 **Notification Service** — Email/SMS notifications (replaces ConsoleEmailService)
- 📊 **Analytics Service** — User activity and platform metrics

---

<p align="center">
  <b>DOL Platform</b> — Built with ❤️ using .NET 10, Clean Architecture & CQRS
</p>
