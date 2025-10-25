# APMMS - Auto Parts Maintenance Management System
## Figure 4-1: APMMS System Architecture

```
                                    🖥️ 📱
                              User Interface
                              (Web Browser)
                                        │
                                        ▼ HTTP/HTTPS
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              Microsoft Azure                                   │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                        Windows Server                                   │   │
│  │  ┌─────────────────────────────────────────────────────────────────┐   │   │
│  │  │                    IIS 10.0                                    │   │   │
│  │  │  ┌─────────────────────────────────────────────────────────┐   │   │   │
│  │  │  │              APMMS Front-End                            │   │   │   │
│  │  │  │  🎮 ASP.NET Core MVC 8.0                               │   │   │   │
│  │  │  │  • Razor Pages                                          │   │   │   │
│  │  │  │  • Controllers                                          │   │   │   │
│  │  │  │  • ViewModels                                           │   │   │   │
│  │  │  │  • Bootstrap UI                                         │   │   │   │
│  │  │  └─────────────────────────────────────────────────────────┘   │   │   │
│  │  │                                        │                        │   │   │
│  │  │                                        ▼ HTTP/HTTPS             │   │   │
│  │  │  ┌─────────────────────────────────────────────────────────┐   │   │   │
│  │  │  │              APMMS Back-End                            │   │   │   │
│  │  │  │  🚀 ASP.NET Core Web API 8.0                         │   │   │   │
│  │  │  │  • RESTful APIs                                       │   │   │   │
│  │  │  │  • JWT Authentication                                 │   │   │   │
│  │  │  │  • Entity Framework Core                              │   │   │   │
│  │  │  │  • AutoMapper                                         │   │   │   │
│  │  │  │  • Swagger/OpenAPI                                    │   │   │   │
│  │  │  └─────────────────────────────────────────────────────────┘   │   │   │
│  │  │                                        │                        │   │   │
│  │  │                                        ▼ TDS/HTTPS              │   │   │
│  │  │  ┌─────────────────────────────────────────────────────────┐   │   │   │
│  │  │  │              SQL Server 2019                           │   │   │   │
│  │  │  │  🗄️ Database Engine                                   │   │   │   │
│  │  │  │  • CarMaintenanceDbContext                            │   │   │   │
│  │  │  │  • Entity Models                                      │   │   │   │
│  │  │  │  • Stored Procedures                                  │   │   │   │
│  │  │  └─────────────────────────────────────────────────────────┘   │   │   │
│  │  └─────────────────────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼ HTTPS
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            External Services                                   │
├─────────────────────────────────────────────────────────────────────────────────┤
│  ☁️ Cloudinary API            📧 Email Service            🔐 JWT Service        │
│  • Image Upload              • SMTP Integration          • Token Generation     │
│  • File Storage              • Notification System       • Authentication       │
│  • CDN Delivery              • Report Delivery           • Authorization        │
└─────────────────────────────────────────────────────────────────────────────────┘
```

## Technology Stack Summary

### Cloud Platform
- **Microsoft Azure** - Cloud hosting platform

### Operating System & Web Server
- **Windows Server** - Operating system
- **IIS 10.0** - Web server

### Frontend
- **ASP.NET Core MVC 8.0** - Web framework
- **Razor Pages** - View engine
- **Bootstrap** - UI framework

### Backend
- **ASP.NET Core Web API 8.0** - API framework
- **Entity Framework Core** - ORM
- **JWT Authentication** - Security
- **AutoMapper** - Object mapping
- **Swagger/OpenAPI** - API documentation

### Database
- **SQL Server 2019** - Database engine
- **TDS/HTTPS** - Database protocol

### External Services
- **Cloudinary API** - Image management
- **Email Service** - Notification system
- **JWT Service** - Authentication service

### Communication Protocols
- **HTTP/HTTPS** - Web communication
- **TDS/HTTPS** - Database communication
- **HTTPS** - External service communication
