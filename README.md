# 🧑‍💼 Employee Management System (HRMS)

This is an ASP.NET Core Web API project developed as part of a backend assessment.
It serves as a simplified Human Resource Management System (HRMS) that manages employees, leave requests, salary processing, and role-based workflows.

## Features

- JWT Authentication & Role-based Authorization
- CQRS + Mediator Pattern using MediatR
- Postgres and Entity Framework Core (EF Core) with repository abstractions
- Employee Management (Add, Update, List, Filter by Department, etc.)
- Leave Request Workflow (Employee request → Manager approval → HR view)
- Employee Salary Calculation Rules
- FluentValidation for input validation
- Middleware for logging and exception handling  
- Swagger/OpenAPI Documentation  
- Unit & Integration Tests using xUnit and Moq
- In-Memory Database Support for testing


## Setup Instructions

1. Clone the repository: `git clone git@github.com:azmirsabir/ https://github.com/azmirsabir/HRMS.git && cd HRMS`  
2. Create a Postgres database: `employeems`
3. Open `appsettings.json` and update connection string:  
   `"IdentityConnection": "Host=127.0.0.1;Database=employeems;Username=jihad;Password=rootroot"`  
4. (Optional) Install EF Core CLI: `dotnet tool install --global dotnet-ef`  
5. Apply migrations: `dotnet ef database update`  
6. Run the app: `dotnet run`  
7. Access Swagger UI at `http://localhost:5001/swagger/index.html` 

## 📂 Project Structure

EmployeeManagementSystem/
│
├── 📦 API/
│ └── Web API Layer
│ - Controllers
│ - DI Setup / Program.cs
│
├── 📦 Application/
│ └── CQRS & Application Layer
│ - Handlers (Commands & Queries)
│ - Interfaces & DTOs
│
├── 📦 Core/
│ └── Domain Layer
│ - Entities (User, Employee, Leave, Department)
│ - Enums & Value Objects
│
├── 📦 Infrastructure/
│ └── Infrastructure Layer
│ - EF Core Context & Migrations
│ - Repositories & Services
│
├── 📦 Test/
│ └── Unit & Integration Tests
│ - xUnit & Moq-based
│ - Test Data Seeds
│
├── 📄 EmployeeManagementSystem.sln
└── 📄 README.md

### ✅ Notes:

- **API** → Only handles controllers, DI, and configuration.  
- **Application** → Contains CQRS commands/queries, interfaces, validators, and contracts.  
- **Core** → Domain entities and business rules, independent of infrastructure.  
- **Infrastructure** → Database, EF Core, service implementations.  
- **Test** → xUnit/Moq tests and data seeding.  

---

## Roles

- Employee: only your own record, request a leave 
- Manager: view department employees; accept/reject leaves
- Auditor : manage employees, see all leaves and employee details, update employee data

## Prerequisites

- .NET 9 SDK  
- Postres  
- dotnet-ef CLI tool  

## License

This is an project intended for assignment purposes only.

## 👤 Author

Azmir Sabir  
GitHub: [@azmirsabir](https://github.com/azmirsabir)
