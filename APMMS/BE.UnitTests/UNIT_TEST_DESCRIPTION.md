# UNIT/INTEGRATION TEST DESCRIPTION - APMMS Backend

## 📋 TỔNG QUAN

Dự án Unit Test và Integration Test cho **APMMS (Auto Parts Maintenance Management System)** Backend được xây dựng bằng **.NET 8.0** và sử dụng **xUnit** framework để đảm bảo chất lượng và độ tin cậy của các service layer.

### Vai trò của Unit/Integration Testing trong dự án

**xUnit** is an open source, community-focused unit testing framework designed for .NET applications. It allows developers to write and execute unit tests and integration tests efficiently, supports various testing patterns including test theories and data-driven tests, and runs on the .NET platform. We use xUnit along with Moq and FluentAssertions to build comprehensive test suites including both unit tests (with mocked dependencies) and integration tests (with InMemory database) for the APMMS Backend services, ensuring code quality, reliability, and maintainability for the vehicle maintenance management system.

### Vai trò của Swagger/OpenAPI trong dự án

**Swagger (OpenAPI)** is an open source, industry-standard specification and toolset designed for describing, documenting, and testing RESTful APIs. It allows developers to automatically generate interactive API documentation, test API endpoints directly from the browser, supports various authentication methods including JWT Bearer tokens, and runs on the .NET platform. We use Swagger with SwaggerUI to provide interactive API documentation and testing interface for the APMMS Backend API, enabling developers and testers to explore endpoints, test requests with authentication, validate request/response schemas, and ensure API correctness for the vehicle maintenance management system.

### Vai trò của GitHub trong dự án

**GitHub** is an open source, web-based version control and collaboration platform designed for software development and project management. It allows developers to create and manage issues, log bugs, track project tasks, supports version control with Git, pull requests, code reviews, and runs on cloud-based infrastructure. We use GitHub Issues to manage bug tracking and issue logging for the APMMS project, enabling developers and team members to report bugs, document issues, track bug status, assign tasks, and maintain a centralized record of all project issues and bugs for the vehicle maintenance management system.

### Vai trò của Chrome DevTools trong dự án

**Chrome DevTools** is a built-in, web-based developer toolset designed for debugging and inspecting web applications in the Google Chrome browser. It allows developers to view console logs, inspect HTML elements, debug JavaScript code, monitor network requests, supports performance profiling, responsive design testing, and runs directly in the Chrome browser. We use Chrome DevTools to view logs, inspect elements, debug frontend issues, monitor API calls, and analyze application performance for the APMMS Frontend, enabling developers to identify and fix bugs, optimize rendering, validate CSS styles, and ensure proper functionality for the vehicle maintenance management system.

### Vai trò của Google Sheets trong dự án

**Google Sheets** is a cloud-based, collaborative spreadsheet application designed for data management and documentation. It allows developers and testers to manage test cases, record test results, track test traceability, supports real-time collaboration, data filtering, conditional formatting, and runs on web-based platform. We use Google Sheets to manage test cases, document test results, maintain test traceability matrices, track test coverage, and organize testing documentation for the APMMS project, enabling the testing team to document test scenarios, record execution results, link requirements to test cases, and ensure comprehensive test management for the vehicle maintenance management system.

---

## 🛠️ CÔNG NGHỆ VÀ FRAMEWORK

### Test Framework
- **xUnit 2.6.2** - Framework chính cho unit testing
- **xunit.runner.visualstudio 2.5.3** - Test runner cho Visual Studio
- **Microsoft.NET.Test.Sdk 17.10.0** - Test SDK

### Mocking & Assertion
- **Moq 4.20.70** - Mocking framework cho dependencies
- **FluentAssertions 6.12.0** - Fluent assertion library

### Database Testing
- **Microsoft.EntityFrameworkCore.InMemory 8.0.19** - In-memory database cho integration testing
- **Microsoft.EntityFrameworkCore 8.0.19** - EF Core

### Mapping
- **AutoMapper 12.0.1** - Object mapping

### Code Coverage
- **coverlet.collector 6.0.0** - Code coverage tool

### API Documentation & Testing
- **Swashbuckle.AspNetCore (Swagger/OpenAPI)** - API documentation and testing tool
- **SwaggerUI** - Interactive API testing interface

### Version Control & Issue Management
- **GitHub Issues** - Bug tracking and issue logging platform
- **Git** - Distributed version control system

### Development & Debugging Tools
- **Chrome DevTools** - Browser-based debugging tool for viewing logs and inspecting elements

### Test Management
- **Google Sheets** - Test case management, test result documentation, and test traceability tracking

---

## 🖥️ TEST ENVIRONMENT

| ID | Environment | Details |
|----|-------------|---------|
| 1 | **Hardware** | • Processor: Intel i5 or AMD Ryzen 5 or higher<br>• RAM: 8 GB or more<br>• Storage: 100 GB free disk space (SSD recommended) |
| 2 | **Software** | • Operating System: Windows 10/11<br>• Database: SQL Server 2019 or higher<br>• Backend: .NET 8.0 Runtime/SDK<br>• Frontend: ASP.NET Core MVC, Razor Pages, Bootstrap 5<br>• Browser: Chrome (latest versions)<br>• Development Tools: Visual Studio 2022<br>• Image Storage: Cloudinary<br>• Email Service: SMTP (Gmail)<br>• PDF Generation: QuestPDF<br>• Excel Processing: EPPlus |
| 3 | **Infrastructure** | • CI/CD pipeline using GitHub Actions<br>• Cloud hosting: Microsoft Azure |
| 4 | **Mock Data** | • 10+ Employee accounts (Accountant, Technician, Warehouse Keeper, Consulter)<br>• 20+ AutoOwner accounts<br>• 30+ Car records<br>• 50+ Maintenance Tickets<br>• 50+ Service Schedules<br>• 30+ Vehicle Check-ins<br>• 100+ Components (various types)<br>• 20+ Service Packages |

---

## 📁 CẤU TRÚC DỰ ÁN

```
BE.UnitTests/
├── Services/                          # Test classes cho các services
│   ├── EmployeeServiceTests.cs       # 66 test cases
│   ├── MaintenanceTicketServiceTests.cs  # 102 test cases
│   ├── ServiceScheduleServiceTests.cs    # 105 test cases
│   ├── VehicleCheckinServiceTests.cs     # 33 test cases
│   └── AutoOwnerServiceTests.cs          # 10 test cases
├── Helpers/                          # Helper classes
│   ├── MockDataBuilder.cs           # Builder pattern cho mock data
│   └── DbContextMock.cs             # DbContext mocking utilities
├── MAINTENANCE_TICKET_TEST_PLAN.md  # Test plan cho MaintenanceTicket
├── SERVICES_EVALUATION.md           # Đánh giá các services
└── UNIT_TEST_DESCRIPTION.md         # File này
```

---

## 📊 THỐNG KÊ TEST CASES

| Service | Số Test Cases | Trạng Thái | Ghi Chú |
|---------|--------------|------------|---------|
| **ServiceScheduleService** | 105 | ✅ Complete | Đầy đủ Boundary Value & Equivalence Partitioning |
| **MaintenanceTicketService** | 102 | ✅ Complete | Status workflow, Technician assignment |
| **EmployeeService** | 66 | ✅ Complete | Validation rules, CRUD operations |
| **VehicleCheckinService** | 33 | ✅ Complete | Vehicle check-in workflow |
| **AutoOwnerService** | 10 | ✅ Complete | Basic CRUD operations |
| **TỔNG CỘNG** | **316** | ✅ | |

---

## 🎯 PHƯƠNG PHÁP TESTING

### 1. Boundary Value Testing (BVT)
Kiểm tra các giá trị biên và ranh giới của input:
- **IDs**: `0`, `1`, `-1`, `long.MaxValue`
- **Dates**: `DateTime.UtcNow`, `DateTime.MinValue`, `DateTime.MaxValue`
- **Pagination**: `page = 0`, `page = 1`, `pageSize = 0`, `pageSize = 1`
- **Status Codes**: `null`, `""`, valid statuses, invalid statuses
- **Quantities**: `0`, negative values, `int.MaxValue`

### 2. Equivalence Partitioning (EP)
Phân chia input thành các nhóm tương đương:
- **Valid/Invalid IDs**: Existing vs non-existing entities
- **Valid/Invalid Status Transitions**: Allowed vs disallowed transitions
- **Valid/Invalid Dates**: Future vs past dates
- **Valid/Invalid Formats**: Email, phone, citizen ID, tax code

### 3. Test Naming Convention
```
{MethodName}_Should{ExpectedBehavior}_When{Condition}
```

**Ví dụ:**
- `CreateAsync_WithValidData_ShouldReturnSuccess`
- `UpdateAsync_WithNonExistentId_ShouldReturnNull`
- `DeleteAsync_WithInvalidId_ShouldReturnFalse`

---

## 🔍 CHI TIẾT CÁC TEST CLASSES

### 1. EmployeeServiceTests (66 test cases)

#### Phạm vi test:
- ✅ **CreateAsync** (15 test cases)
  - Valid data creation
  - Default status code (ACTIVE)
  - Invalid role/branch IDs
  - Duplicate username validation
  - Email/Phone/CitizenId/TaxCode format validation
  - Age validation (minimum 18)
  - Code generation (role-based prefixes)
  - Audit log creation

- ✅ **UpdateAsync** (10 test cases)
  - Valid updates
  - Non-existent ID handling
  - Duplicate username prevention
  - Empty password handling (keep original)
  - Null role/branch ID handling (keep original)
  - Audit log creation

- ✅ **GetByIdAsync** (2 test cases)
  - Valid ID returns employee
  - Invalid ID returns null

- ✅ **GetAllAsync** (2 test cases)
  - Returns all employees
  - Filtering by status/role

- ✅ **DeleteAsync** (2 test cases)
  - Soft delete with valid ID
  - Returns false for invalid ID

- ✅ **UpdateStatusAsync** (3 test cases)
  - Valid status updates
  - Invalid status handling
  - Non-existent ID handling

- ✅ **GetWithFiltersAsync** (5 test cases)
  - Pagination
  - Search term filtering
  - Status filtering
  - Role filtering

- ✅ **FilterAsync** (2 test cases)
  - IsDelete filter
  - RoleId filter

- ✅ **GetProfileAsync** (2 test cases)
  - Valid user ID
  - Invalid user ID

- ✅ **UpdateProfileAsync** (3 test cases)
  - Valid profile update
  - Invalid user ID
  - Empty fields handling

- ✅ **Validation Tests** (20 test cases)
  - Phone format (valid/invalid)
  - CitizenId format (12 digits)
  - TaxCode format (10 digits)
  - Email format
  - DOB age validation

---

### 2. MaintenanceTicketServiceTests (102 test cases)

#### Phạm vi test:
- ✅ **CreateMaintenanceTicketAsync** (15 test cases)
  - Boundary: CarId = 0, 1, -1, long.MaxValue
  - Valid/invalid car existence
  - Service category validation
  - Priority level validation

- ✅ **CreateFromVehicleCheckinAsync** (12 test cases)
  - VehicleCheckin ID validation
  - Technician assignment
  - Primary technician selection

- ✅ **UpdateMaintenanceTicketAsync** (10 test cases)
  - Valid updates
  - Non-existent ticket handling
  - Field updates

- ✅ **GetMaintenanceTicketByIdAsync** (8 test cases)
  - Valid ID returns ticket
  - Invalid ID handling
  - Related entities loading

- ✅ **GetAllMaintenanceTicketsAsync** (10 test cases)
  - Pagination
  - Branch filtering
  - Empty results

- ✅ **UpdateStatusAsync** (15 test cases) ⭐
  - Status transitions: PENDING → ASSIGNED → IN_PROGRESS → COMPLETED
  - Invalid transitions prevention
  - History log creation

- ✅ **AssignTechnicianAsync** (10 test cases)
  - Valid technician assignment
  - Status auto-update to ASSIGNED
  - Invalid technician handling

- ✅ **AddTechniciansAsync** (12 test cases) ⭐
  - Multiple technician assignment
  - Status validation
  - Primary technician selection

- ✅ **RemoveTechniciansAsync** (8 test cases)
  - Technician removal
  - Status auto-update to PENDING
  - Status validation

- ✅ **StartMaintenanceAsync** (6 test cases)
  - Status validation (must be ASSIGNED)
  - Technician requirement

- ✅ **CompleteMaintenanceAsync** (6 test cases)
  - Status validation (must be IN_PROGRESS)
  - TotalReceipt creation

- ✅ **CancelMaintenanceTicketAsync** (6 test cases)
  - Cancellation from various statuses
  - Status validation

---

### 3. ServiceScheduleServiceTests (105 test cases)

#### Phạm vi test:
- ✅ **CreateScheduleAsync** (20 test cases)
  - Date boundary testing (UtcNow, MinValue, MaxValue)
  - Car ownership validation
  - Schedule conflict detection
  - Branch validation

- ✅ **UpdateScheduleAsync** (15 test cases)
  - Valid updates
  - Date validation
  - Conflict detection

- ✅ **GetScheduleByIdAsync** (8 test cases)
  - Valid ID returns schedule
  - Invalid ID handling

- ✅ **GetAllSchedulesAsync** (10 test cases)
  - Pagination
  - Filtering by user/branch/status

- ✅ **GetSchedulesByUserIdAsync** (8 test cases)
  - User filtering
  - Date range filtering

- ✅ **UpdateStatusAsync** (12 test cases)
  - Status transitions
  - Invalid transitions prevention

- ✅ **CancelScheduleAsync** (8 test cases)
  - Cancellation validation
  - Status requirements

- ✅ **Public Booking Flow** (10 test cases)
  - Public schedule creation
  - Auto-owner registration
  - Car registration

- ✅ **Notes Management** (8 test cases)
  - Notes creation
  - Notes updates

- ✅ **Date Range Queries** (6 test cases)
  - Start date filtering
  - End date filtering

---

### 4. VehicleCheckinServiceTests (33 test cases)

#### Phạm vi test:
- ✅ **CreateVehicleCheckinAsync** (10 test cases)
  - Valid check-in creation
  - Car ownership validation
  - Mileage validation
  - Image URL handling

- ✅ **UpdateVehicleCheckinAsync** (8 test cases)
  - Valid updates
  - Status validation
  - Mileage updates

- ✅ **GetVehicleCheckinByIdAsync** (5 test cases)
  - Valid ID returns check-in
  - Invalid ID handling

- ✅ **GetAllVehicleCheckinsAsync** (5 test cases)
  - Pagination
  - Filtering

- ✅ **LinkMaintenanceRequestAsync** (5 test cases)
  - Maintenance request linking
  - Validation

---

### 5. AutoOwnerServiceTests (10 test cases)

#### Phạm vi test:
- ✅ **GetAllAsync** (2 test cases)
  - Returns mapped list
  - Pagination

- ✅ **GetWithFiltersAsync** (2 test cases)
  - Filtering
  - Pagination

- ✅ **GetByIdAsync** (2 test cases)
  - Valid ID
  - Invalid ID

- ✅ **CreateAsync** (2 test cases)
  - Valid creation
  - Validation

- ✅ **UpdateAsync** (2 test cases)
  - Valid update
  - Invalid ID

---

## 🏗️ TEST STRUCTURE

### Test Class Structure
```csharp
public class ServiceNameTests : IDisposable
{
    // Mock dependencies
    private readonly Mock<IRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CarMaintenanceDbContext _context;
    private readonly ServiceName _service;

    // Constructor - Setup mocks and service
    public ServiceNameTests()
    {
        // Initialize mocks
        // Setup InMemory database
        // Create service instance
    }

    // Test methods organized by regions
    #region MethodName Tests
    [Fact]
    public async Task MethodName_ShouldBehavior_WhenCondition()
    {
        // Arrange
        // Act
        // Assert
    }
    #endregion

    // Cleanup
    public void Dispose()
    {
        _context?.Dispose();
    }
}
```

### Test Method Pattern
```csharp
[Fact]
public async Task CreateAsync_WithValidData_ShouldReturnSuccess()
{
    // Arrange
    var dto = MockDataBuilder.CreateRequestDto();
    var entity = MockDataBuilder.CreateEntity();
    
    _mockRepository.Setup(x => x.AddAsync(It.IsAny<Entity>()))
        .Returns(Task.CompletedTask);
    _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<long>()))
        .ReturnsAsync(entity);

    // Act
    var result = await _service.CreateAsync(dto, userId);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(entity.Id);
    
    // Verify
    _mockRepository.Verify(x => x.AddAsync(It.IsAny<Entity>()), Times.Once);
}
```

---

## 🧪 HELPER CLASSES

### MockDataBuilder
Static helper class để tạo mock data với default values:
- `CreateEmployeeRequestDto()` - Tạo EmployeeRequestDto với default values
- `CreateEmployee()` - Tạo Employee entity
- `CreateVehicleCheckinRequestDto()` - Tạo VehicleCheckinRequestDto
- `CreateVehicleCheckin()` - Tạo VehicleCheckin entity
- Và nhiều helper methods khác...

**Ví dụ:**
```csharp
var dto = MockDataBuilder.CreateEmployeeRequestDto(
    username: "employee01",
    roleId: 3,
    branchId: 1
);
```

### DbContextMock
Utilities cho InMemory database setup và teardown.

---

## ✅ TEST COVERAGE

### Coverage Metrics
- **Total Test Cases**: 316
- **Services Covered**: 5 services
- **Methods Covered**: ~50+ methods
- **Test Types**: Unit Tests (with mocked dependencies)

### Coverage Areas
- ✅ **CRUD Operations**: Create, Read, Update, Delete
- ✅ **Validation Logic**: Input validation, business rules
- ✅ **Status Workflows**: Status transitions, state management
- ✅ **Error Handling**: Exception handling, error messages
- ✅ **Edge Cases**: Boundary values, null handling
- ✅ **Integration Points**: Service interactions, repository calls

---

## 🚀 CHẠY TESTS

### Visual Studio
1. Mở Test Explorer (Test → Test Explorer)
2. Chọn test class hoặc test method
3. Click "Run" hoặc "Debug"

### Command Line
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~EmployeeServiceTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Test Results
- ✅ All tests should pass
- ✅ Code coverage report available
- ✅ Test execution time tracked

---

## 📝 BEST PRACTICES

### 1. Test Isolation
- Mỗi test method độc lập
- Sử dụng InMemory database riêng cho mỗi test
- Cleanup trong `Dispose()` method

### 2. Mocking Strategy
- Mock tất cả external dependencies
- Sử dụng `It.IsAny<T>()` cho flexible matching
- Verify method calls với `Verify()`

### 3. Assertion Style
- Sử dụng FluentAssertions cho readable assertions
- Verify cả return values và side effects
- Check exception messages

### 4. Test Data
- Sử dụng MockDataBuilder cho consistent test data
- Avoid hardcoded values
- Use meaningful test data

### 5. Test Organization
- Group tests by method using `#region`
- Use descriptive test names
- Follow AAA pattern (Arrange-Act-Assert)

---

## 🔄 CONTINUOUS INTEGRATION

### CI/CD Integration
Tests có thể được chạy trong CI/CD pipeline:
- **GitHub Actions**: `.github/workflows/test.yml`
- **Azure DevOps**: Pipeline với test task
- **Jenkins**: Test job configuration

### Quality Gates
- ✅ All tests must pass
- ✅ Minimum code coverage: 80%
- ✅ No test warnings
- ✅ Fast execution time

---

## 📚 TÀI LIỆU THAM KHẢO

### Internal Documents
- `MAINTENANCE_TICKET_TEST_PLAN.md` - Test plan cho MaintenanceTicket
- `SERVICES_EVALUATION.md` - Đánh giá các services

### External Resources
- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [EF Core InMemory Provider](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/)

---

## 📋 TEST STRATEGY

### 5.3.4.1 Testing Types

Within the scope of the APMMS project, the team applies many different types of testing to ensure that the system operates correctly, stably and meets the actual needs of users. The main types of testing include:

● **Unit Testing**: Testing the smallest components of the system such as Service, Repository, Controller. For example: verifying that the EmployeeService.CreateAsync method returns a valid employee DTO, that the MaintenanceTicketService.UpdateStatusAsync correctly transitions ticket status, or that the VehicleCheckinService validates mileage correctly.

● **Integration Testing**: Ensuring that modules interact with each other correctly, for example: the Vehicle Check-in process must combine the VehicleCheckin → Car → AutoOwner → MaintenanceTicket flow in the database. Testing service interactions with repositories and database operations using InMemory database.

● **System Testing**: Testing the entire application in a simulated environment that is close to reality, including the Web UI (Razor Pages) and the backend API. Testing end-to-end workflows such as creating a maintenance ticket from vehicle check-in, assigning technicians, and completing maintenance.

● **Acceptance Testing (UAT)**: Representative users (AutoOwner, Employee roles: Accountant, Technician, Warehouse Keeper, Consulter, Admin) participate in testing to confirm that the system meets the business requirements described in the Use Case (vehicle check-in, maintenance ticket management, service scheduling, component inventory, etc.).

● **Regression Testing**: Conducted after each bug fix or feature update, to ensure that previously stable features such as Login/Register, Employee Management, Maintenance Ticket Creation, or Service Schedule Booking still work normally.

● **Performance Testing**: Evaluate the system's load capacity when many users access at the same time, such as when multiple auto owners are booking service schedules simultaneously, or when warehouse keepers are processing stock-in requests concurrently.

● **Security Testing**: Check for security vulnerabilities, especially user authentication using JWT, role-based authorization (Admin/Employee/AutoOwner), to avoid unauthorized access to administrative functions or sensitive data such as maintenance history, financial receipts, or employee information.

### 5.3.4.2 Testing Phases

| ID | Phases | Details |
|----|--------|---------|
| 1 | **Test Planning** | Define the testing objectives, scope, resources and tools. For example, allocate QA personnel for testing User Authentication, Employee Management, Maintenance Ticket Management, Service Schedule Booking, Vehicle Check-in, Component Inventory, and Admin Dashboard. |
| 2 | **Test Design** | Analyze Use Cases to build detailed test scenarios and test cases. Create test cases using Boundary Value Testing and Equivalence Partitioning techniques. Document test cases in Google Sheets for traceability. |
| 3 | **Test Environment Setup** | Configure the test environment to closely resemble the actual deployment environment, including ASP.NET Core backend, Razor Pages frontend, SQL Server database. At the same time, create dummy data (Employee accounts, AutoOwner accounts, Car records, Maintenance Tickets, Service Schedules, Components). |
| 4 | **Test Execution** | Execute the designed test cases: run unit tests for service/repository using xUnit, integration test between API and database using InMemory database, system test on UI. Record results and report errors in GitHub Issues. |
| 5 | **Defect Reporting & Re-testing** | When a defect is detected, the tester records it in the defect management system (GitHub Issues). The developer fixes it, then QA re-tests to confirm that the defect has been fixed. |
| 6 | **Regression Testing** | After fixing a defect or adding a new feature, QA performs regression testing to ensure that old functions such as Employee CRUD operations, Maintenance Ticket workflow, or Service Schedule booking still work normally. |
| 7 | **User Acceptance Testing (UAT)** | Actual users (AutoOwner, Employee roles: Accountant, Technician, Warehouse Keeper, Consulter, Admin) participate in acceptance testing to verify that the system meets business needs for vehicle maintenance management. |
| 8 | **Test Closure** | Synthesize results, evaluate test case coverage, system stability, and prepare a final report. Review test metrics including code coverage, test pass rate, and defect density. |

### 5.3.4.3 Test Schedule

Table below is the Test Schedule for APMMS Project:

| Sprint | Time | Main Testing Activities |
|--------|------|-------------------------|
| **Iter 1** | 19/05/2025 – 20/07/2025 | ● Unit test for core service modules (EmployeeService, MaintenanceTicketService, ServiceScheduleService)<br>● API test using Swagger<br>● Integration Test with InMemory database<br>● System test initially on UI (Vehicle Check-in, Maintenance Ticket Creation) |
| **Iter 2** | 21/07/2025 – 21/08/2025 | ● Unit + Integration test for remaining services<br>● UAT (User Acceptance Testing) with representative groups (AutoOwner, Employee roles, Admin)<br>● Regression test the whole system<br>● Performance and Security testing<br>● Test Closure & Final Report |

### 5.3.4.4 Deliverables

Table below is the Deliverables for APMMS Project:

| Deliverables | Responsibilities | Complete Date |
|--------------|------------------|---------------|
| **Test Plan Document** (target, scope, test strategy, schedule) | Project Manager + Tester | Iter 1 (19/05 – 20/07/2025) |
| **Test Case Specification** | Tester | Iter 1 (19/05 – 20/07/2025) |
| **Test Environment Setup Report** (test environment configuration, simulation data) | Tester + DevOps | Iter 1 (19/05 – 20/07/2025) |
| **Unit Test Report** | Developer + Tester | Iter 1 (19/05 – 20/07/2025) |
| **Integration Test Report** | Tester | Iter 1 (19/05 – 20/07/2025) |
| **System Test Report** (end-to-end on Web + API) | Tester | Iter 2 (21/07 – 21/08/2025) |
| **Non-functional Test Report** (Performance, Security, Usability) | Tester | Iter 2 (21/07 – 21/08/2025) |
| **UAT Report** (User Acceptance Test with AutoOwner, Employee roles, Admin) | Tester | Iter 2 (21/07 – 21/08/2025) |
| **Final Test Summary Report** (summarizing results, bug logs, system quality assessment) | Project Manager | Iter 2 (21/07 – 21/08/2025) |

---

## 🔧 TESTING FRAMEWORK

### 5.4.1.1 Testing Framework

● **For Unit testing and Integration testing**, we use the **xUnit.net** testing framework, combined with **FluentAssertions** for expressive assertions and **Moq** for mocking dependencies.

● **All testing frameworks and libraries** are installed into the APMMS solution automatically via NuGet package manager. The test project references the main BE project to access service classes, repositories, and models.

● **All automation test scripts** are created manually and saved under the **BE.UnitTests** project. Unit tests are stored in the **BE.UnitTests** project, organized into:
  - **Services/** folder: Contains test classes for all service layer components (EmployeeServiceTests, MaintenanceTicketServiceTests, ServiceScheduleServiceTests, VehicleCheckinServiceTests, AutoOwnerServiceTests)
  - **Helpers/** folder: Contains helper classes for test data creation and database mocking (MockDataBuilder, DbContextMock)

**Test Directory Structure:**
```
BE.UnitTests/
├── Services/                          # Test classes cho các services
│   ├── EmployeeServiceTests.cs       # 66 test cases
│   ├── MaintenanceTicketServiceTests.cs  # 102 test cases
│   ├── ServiceScheduleServiceTests.cs    # 105 test cases
│   ├── VehicleCheckinServiceTests.cs     # 33 test cases
│   └── AutoOwnerServiceTests.cs          # 10 test cases
├── Helpers/                          # Helper classes
│   ├── MockDataBuilder.cs           # Builder pattern cho mock data
│   └── DbContextMock.cs             # DbContext mocking utilities
└── BE.UnitTests.csproj               # Test project configuration
```

● **Unit tests** focus on individual methods in service classes. They verify business logic in isolation using Moq for dependencies (repositories, other services) and FluentAssertions for readable assertions. For example, testing EmployeeService.CreateAsync method to verify employee creation logic, validation rules, and code generation.

● **Integration tests** use InMemory database (Entity Framework Core InMemory provider) to test service interactions with database operations. They verify that services correctly interact with repositories and that data is properly persisted and retrieved.

● **Test data management** is handled through MockDataBuilder helper class, which provides factory methods to create test entities and DTOs with default or custom values, ensuring consistent and maintainable test data.

---

## 🎯 KẾT LUẬN

Dự án Unit Test của APMMS Backend đã được xây dựng với:
- ✅ **316 test cases** covering 5 major services
- ✅ **Comprehensive testing** với Boundary Value Testing và Equivalence Partitioning
- ✅ **High code coverage** cho critical business logic
- ✅ **Well-organized structure** với helper classes và mock builders
- ✅ **Best practices** trong test design và implementation

**Status**: ✅ Production Ready

---

**Last Updated**: 2025
**Maintained By**: APMMS Development Team

