# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SSRBusiness.NET10 is a modernized business application library migrated from VB.NET Framework 4.8 to C# .NET 10. It manages acquisitions and letter agreements for an oil & gas business, using Entity Framework Core with the Repository pattern for data access.

## Build and Development Commands

```bash
# Restore dependencies and build
dotnet restore
dotnet build

# Entity Framework operations
dotnet ef database update                    # Apply migrations
dotnet ef migrations add <MigrationName>     # Create new migration
dotnet ef dbcontext scaffold                 # Regenerate entities (use scaffold-database.sh instead)

# Database scaffolding (preferred method)
./scaffold-database.sh                       # Generate all entities from database

# Project structure verification
./check-conversion-status.sh                 # Check migration progress
python verify_entities.py                   # Validate entity relationships
```

## Architecture Overview

### Core Patterns
- **Repository Pattern**: All data access through `Repository<T>` base class
- **Entity Framework Core 10**: Modern ORM replacing LINQ to SQL
- **Async/Await**: All database operations are asynchronous
- **Nullable Reference Types**: Enabled throughout the project

### Key Directories
- `Data/`: Contains `SsrDbContext` - the main EF Core DbContext
- `Entities/`: Database entity models (auto-generated from scaffold)
- `BusinessClasses/`: Repository implementations for business logic
- `BusinessFramework/`: Base `Repository<T>` class and shared patterns
- `Interfaces/`: Service contracts and abstractions
- `Support/`: Utility classes (SaltedHash, DTOs, etc.)
- `Migrations/`: EF Core database migrations

### Database Context Structure
The `SsrDbContext` contains 70+ DbSets organized by domain:
- **Acquisition Domain**: `Acquisitions`, `AcquisitionBuyers`, `AcquisitionCounties`, etc.
- **Letter Agreement Domain**: `LetterAgreements`, `LetterAgreementSellers`, etc.
- **Lookup Tables**: `Buyers`, `Counties`, `Operators`, `Referrers`, `States`
- **User Management**: `Users`, `UserRoles`, `Roles`
- **Reporting**: Various report-specific entities
- **Configuration**: `ApplicationSettings`, `DisplayFields`, etc.

## Development Workflow

### Entity Scaffolding
1. Update connection string in `appsettings.json`
2. Run `./scaffold-database.sh` to generate all entity classes
3. Review generated entities in `Entities/Generated/`
4. Build project to verify scaffolding success

### Creating New Repositories
Follow the pattern established in `UserRepository.cs`:

```csharp
public class ExampleRepository : Repository<Example>
{
    private readonly SsrDbContext _ssrContext;

    public ExampleRepository(SsrDbContext context) : base(context)
    {
        _ssrContext = context;
    }

    public async Task<Example?> LoadByIdAsync(int id)
    {
        return await SingleOrDefaultAsync(e => e.Id == id);
    }
}
```

### Migration from VB.NET Pattern
The project follows a systematic conversion pattern:
- VB.NET `SsrBusinessObject<T>` → C# `Repository<T>`
- VB.NET `Function() As Type` → C# `async Task<Type> MethodAsync()`
- VB.NET `Me.Entity` → Direct return or local variables
- VB.NET synchronous operations → C# async/await

### Password Authentication
Uses backward-compatible SHA1 hashing via `SaltedHash` class:
```csharp
// For compatibility with existing VB.NET passwords
var hash = SaltedHash.Create(password, useSha1ForCompatibility: true);
var isValid = hash.Verify(inputPassword);
```

## Key Technologies

### Core Framework
- **.NET 10.0** (LTS): Target framework
- **Entity Framework Core 10.0.1**: ORM and data access
- **Microsoft SQL Server**: Primary database (Azure-hosted)

### Azure Integration
- **Azure.Storage.Blobs**: Blob storage operations
- **Azure.Storage.Files.Shares**: File share access
- File services in `AzureBlobFileService.cs` and `AzureFileShareFileService.cs`

### Document Processing
- **DocumentFormat.OpenXml**: Office document manipulation
- **FastReport.OpenSource**: Report generation and PDF export
- **DocSharp.Binary**: Custom document processing (external dependency)

### Code Analysis
- **Microsoft.CodeAnalysis**: VB.NET, C#, and common analysis tools
- Used for code conversion and analysis during migration

## Configuration

### Database Connection
Connection string in `appsettings.json` must be configured before running scaffold script:
```json
{
  "ConnectionStrings": {
    "SanSabaConnection": "Server=YOUR_SERVER;Database=SanSaba;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

### Project References
External dependencies on DocSharp.Binary projects:
- `DocSharp.Binary.Common`
- `DocSharp.Binary.Doc`

## Migration Status

The project is actively being migrated from VB.NET. Track progress via:
- `MIGRATION_CHECKLIST.md`: Detailed conversion checklist
- `MIGRATION_SUMMARY.md`: High-level migration overview
- `DOTNET10_UPGRADE.md`: .NET 10 specific upgrade notes

### Completed Components
- ✅ Base `Repository<T>` framework
- ✅ `UserRepository` with authentication
- ✅ `SaltedHash` utility class
- ✅ Entity Framework Core setup
- ✅ Database scaffolding infrastructure

### In Progress
- Various repository implementations in `BusinessClasses/`
- Cloud file service abstractions
- Report generation components

## Important Notes

### Security Considerations
- Database connection string contains credentials - treat as sensitive
- SHA1 password hashing maintained for backward compatibility
- Consider migrating to stronger hashing for new passwords

### Development Guidelines
- Always use async/await for database operations
- Follow Repository pattern established in `BusinessFramework/`
- Use Entity Framework Include() for related data loading
- Maintain compatibility with existing VB.NET password hashes
- Generate entities via scaffold script rather than manual creation