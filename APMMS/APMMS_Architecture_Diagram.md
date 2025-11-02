# APMMS - Auto Parts Maintenance Management System Architecture

## System Overview
APMMS là hệ thống quản lý bảo dưỡng xe và linh kiện với kiến trúc 3-tier: Frontend (MVC), Backend (Web API), và Database.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                                CLIENT LAYER                                    │
├─────────────────────────────────────────────────────────────────────────────────┤
│  🌐 Web Browser                    📱 Mobile App (Future)                      │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            FRONTEND LAYER (FE)                                 │
├─────────────────────────────────────────────────────────────────────────────────┤
│  🎮 MVC Controllers          │  📄 Views (Razor)        │  📊 ViewModels        │
│  • HomeController            │  • .cshtml Views         │  • ComponentViewModel │
│  • AuthController            │  • Shared Layouts        │  • MaintenanceTicket  │
│  • ComponentsController      │  • Bootstrap UI          │  • EmployeeViewModel  │
│  • MaintenanceTicketsCtrl    │                          │                       │
│  • EmployeeController        │                          │                       │
│  • BranchController          │                          │                       │
├─────────────────────────────────────────────────────────────────────────────────┤
│  🔧 Services                 │  🔌 Adapters             │  🎨 Static Assets     │
│  • ComponentService          │  • ApiAdapter            │  • CSS Files          │
│  • AuthService               │  • HTTP Client           │  • JavaScript Files   │
│  • UserService               │  • API Communication     │  • Images             │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            BACKEND LAYER (BE)                                  │
├─────────────────────────────────────────────────────────────────────────────────┤
│  🚀 API Controllers          │  💼 Business Logic       │  🗄️ Data Access       │
│  • ComponentController       │  • ComponentService      │  • ComponentRepository│
│  • MaintenanceTicketCtrl     │  • MaintenanceTicketSvc  │  • MaintenanceTicket  │
│  • EmployeeController        │  • EmployeeService       │  • EmployeeRepository │
│  • BranchController          │  • AuthService           │  • UserRepository     │
│  • AuthController            │                          │                       │
│  • ImageController           │                          │                       │
├─────────────────────────────────────────────────────────────────────────────────┤
│  📋 DTOs                     │  🏗️ Models & Context     │  🔐 Security          │
│  • Component DTOs            │  • CarMaintenanceDbContext│  • JWT Authentication │
│  • MaintenanceTicket DTOs    │  • Entity Models         │  • Cloudinary Service │
│  • Employee DTOs             │  • AutoMapper            │  • FluentValidation   │
│  • Auth DTOs                 │  • Swagger/OpenAPI       │                       │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            DATABASE LAYER                                      │
├─────────────────────────────────────────────────────────────────────────────────┤
│  🗃️ SQL Server Database                                                         │
├─────────────────────────────────────────────────────────────────────────────────┤
│  📊 Core Tables              │  🔧 Maintenance Tables   │  🏢 Supporting Tables │
│  • user                      │  • maintenance_ticket    │  • status_lookup      │
│  • branch                    │  • service_task          │  • role               │
│  • car                       │  • vehicle_checkin       │  • permission         │
│  • component                 │  • ticket_component      │  • address            │
│  • type_component            │                          │  • province           │
│                              │                          │  • ward               │
├─────────────────────────────────────────────────────────────────────────────────┤
│  💼 Business Tables                                                             │
│  • service_package           • total_receipt            • feedback              │
│  • history_log                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            EXTERNAL SERVICES                                   │
├─────────────────────────────────────────────────────────────────────────────────┤
│  ☁️ Cloudinary API            📧 Email Service (Future)                         │
└─────────────────────────────────────────────────────────────────────────────────┘
```

## Data Flow Diagram

```
User Request → Frontend Controller → ViewModel → View → JavaScript → API Adapter
     ↓
Backend API Controller → Service Layer → Repository Layer → Database Context
     ↓
SQL Server Database → Response → DTOs → AutoMapper → JSON Response
     ↓
Frontend Service → ViewModel → View → User Interface
```

## Technology Stack

### Frontend (FE)
- **Framework**: ASP.NET Core MVC
- **UI**: Razor Views (.cshtml)
- **Styling**: Bootstrap, CSS3
- **JavaScript**: jQuery, Vanilla JS
- **Authentication**: Cookie Authentication
- **Session Management**: In-Memory Cache

### Backend (BE)
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT Bearer Token
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI
- **Image Upload**: Cloudinary Service

### Database
- **RDBMS**: SQL Server
- **Approach**: Database First
- **Connection**: Entity Framework Core

## Key Features

### 1. Authentication & Authorization
- JWT-based authentication for API
- Cookie-based authentication for MVC
- Role-based access control
- Session management

### 2. Component Management
- CRUD operations for components
- Image upload functionality
- Stock management
- Status tracking (ACTIVE/DISABLED)

### 3. Maintenance Ticket System
- Ticket creation and assignment
- Service task management
- Technician assignment
- Status tracking
- Cost estimation

### 4. Employee Management
- User management
- Role assignment
- Branch-based access
- Profile management

### 5. Reporting & Analytics
- Maintenance reports
- Component usage tracking
- Financial reports
- History logging

## Data Flow

1. **User Request** → Frontend Controller
2. **Frontend Controller** → ViewModel → View
3. **View** → JavaScript → API Adapter
4. **API Adapter** → Backend API Controller
5. **API Controller** → Service Layer
6. **Service Layer** → Repository Layer
7. **Repository Layer** → Database Context
8. **Database Context** → SQL Server Database

## Security Features

- JWT token validation
- CORS configuration
- Input validation
- SQL injection prevention
- XSS protection
- Session timeout
- Role-based authorization

## Deployment Architecture

- **Frontend**: IIS/NGINX
- **Backend**: IIS/Kestrel
- **Database**: SQL Server
- **File Storage**: Cloudinary
- **Load Balancer**: Application Gateway (optional)
