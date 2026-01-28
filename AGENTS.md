# AGENTS.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

**SSRBusiness** is a .NET 10 C# library migrated from legacy VB.NET .NET Framework 4.8. It's an enterprise real estate acquisition and letter agreement management system with ~60 database entities and complex business logic for handling land transactions, buyer/seller management, operator data, and county information.

## Build and Development Commands

### Restore Dependencies
```bash
dotnet restore
```

### Build the Project
```bash
dotnet build
```

### Build with Specific Configuration
```bash
dotnet build --configuration Release
dotnet build --configuration Debug
```

### Clean Build Artifacts
```bash
dotnet clean
```

## Architecture Overview

### High-Level Structure

The codebase follows a **Repository Pattern** with Entity Framework Core for database access:

- **BusinessFramework/** - Contains `Repository<T>` base class providing generic CRUD operations (`GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `SaveChangesAsync`, `FindAsync`, `SingleOrDefaultAsync`, etc.)
- **BusinessClasses/** - Domain-specific repositories (e.g., `AcquisitionRepository`, `BuyerRepository`) that inherit from `BaseRepository<T>` and implement complex business queries
- **Data/** - `SsrDbContext` (DbContext) managing all entity mappings and configurations for ~60 entities
- **Entities/** - Auto-generated entity classes from database scaffolding (created via `scaffold-database.sh`)
- **Support/** - Utility classes like `SaltedHash` (password hashing with SHA1/SHA256 support) and `LookupListItem` DTOs
- **Interfaces/** - Abstractions like `IFileService` for file operations (Azure Blob Storage, Azure File Share)

### Database Context Initialization

The `SsrDbContext` is configured with:
- Entity mappings for all business domain entities (Acquisitions, LetterAgreements, Buyers, Operators, Counties, etc.)
- Navigation properties with eager loading includes for complex entity graphs
- Model configurations in `OnModelCreating()` (e.g., ignored navigation properties due to type mismatches, foreign key customizations)

### Key Implementation Patterns

**Repository Methods:**
All repositories follow async/await patterns. Methods typically end with `Async`:
- `GetByIdAsync(params)` - Fetch by primary key
- `FindAsync(predicate)` - Query with lambda expression
- `SingleOrDefaultAsync(predicate)` - Get single entity or null
- `AddAsync(entity)` - Add new entity
- `DeleteAsync(predicate)` - Delete matching entities
- `SaveChangesAsync()` - Persist changes to database

**Complex Queries:**
Repositories like `AcquisitionRepository` implement sophisticated filtering with expression trees (`ApplyDateFilter`, `ApplyDealStatusFilter`) to support dynamic report generation and filtered lookups.

**File Services:**
`IFileService` implementations handle both temporary file operations and permanent storage (Azure Blob Storage, Azure File Share). Used for document uploads and report generation.

## Important Implementation Details

### Password Hashing (SaltedHash)
- **Backward Compatible**: Uses SHA1 by default for existing VB.NET passwords
- **Verify Password**: `var hash = SaltedHash.Create(storedSalt, storedPassword); hash.Verify(inputPassword)`
- **Create New Hash**: `var newHash = SaltedHash.Create(userPassword, useSha1ForCompatibility: false)` for new passwords (SHA256)

### Entity Relationships and Includes
Complex entities require explicit `.Include()` statements for related data. For example, `LoadAcquisitionByAcquisitionIDAsync` includes 10+ related entity graphs. Missing includes will result in lazy-loading issues or null reference exceptions.

### Known Data Model Issues
- **Acquisition.LandMan** navigation property is ignored in `OnModelCreating()` due to type mismatch (LandManID is `int?` but User.UserId is `string`)
- **DocumentTemplate relationship** uses `DocumentTypeCode` foreign key instead of standard navigation

### EF Core Migrations
Migrations are stored in the `Migrations/` directory. To add a new migration:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Connection String
Located in `appsettings.json`. Points to Azure SQL Server (ssr.database.windows.net). Ensure credentials are properly set before running.

## Dependency Injection Setup

When integrating repositories in a host application (e.g., ASP.NET Core):
```csharp
services.AddDbContext<SsrDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SanSabaConnection")));

services.AddScoped<AcquisitionRepository>();
services.AddScoped<BuyerRepository>();
// etc.
```

## Testing Considerations

- No dedicated test project exists yet
- When creating tests, ensure database is accessible or mock the `SsrDbContext`
- Integration tests should use actual database with test data
- Unit tests for repositories can mock DbSet using `Moq` or similar

## File Service Implementations

Two implementations exist:
- **AzureBlobFileService** - Uses Azure Blob Storage for permanent file storage
- **AzureFileShareFileService** - Uses Azure File Share for shared access

Both implement `IFileService` and handle temporary file management with cleanup capabilities.

## Legacy Migration Notes

This codebase is a modernization of the original VB.NET .NET Framework 4.8 SSRBusiness library. Key conversions:
- VB `Function()` → C# `async Task<T>()`
- LINQ to SQL → Entity Framework Core
- Custom `SsrBusinessObject<T>` base class → Generic `Repository<T>` pattern
- Synchronous methods → All async/await

See `README.md` for detailed migration patterns and conversion examples.

## Solution Structure

- **SSRBusiness.csproj** - Main library project (.NET 10, C# 14)
- **SSRReports** - VB.NET reports project (legacy, may require separate handling)
- Both included in `SSRBusiness.sln`

## External Dependencies

Key NuGet packages:
- Microsoft.EntityFrameworkCore 10.0.1 & SqlServer provider
- Azure.Storage.Blobs & Azure.Storage.Files.Shares for file operations
- DocumentFormat.OpenXml 3.0.1 for document processing
- FastReport.OpenSource for reporting
- Microsoft.CodeAnalysis packages for code generation support

## Common Development Tasks

### Adding a New Repository
1. Ensure entity exists in `Entities/`
2. Create `BusinessClasses/[Entity]Repository.cs` inheriting from `BaseRepository<[Entity]>`
3. Implement domain-specific methods using inherited base operations
4. Inject via DI in host application

### Running Migrations
```bash
dotnet ef database update
```

### Scaffolding Database Changes
```bash
chmod +x scaffold-database.sh
./scaffold-database.sh
```

This regenerates entity classes from the database schema. Review changes before committing.

## Documentation Files

- **README.md** - Migration overview, setup guide, architecture, usage examples
- **MIGRATION_SUMMARY.md** - Original migration status and next steps
- **MIGRATION_CHECKLIST.md** - Entity-by-entity migration tracker
- **DOTNET10_UPGRADE.md** - .NET 8 to .NET 10 upgrade notes
