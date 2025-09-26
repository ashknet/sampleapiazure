# Runtime Fixes Applied

## Fixed: BaseEvent Primary Key Error

### Problem
```
System.InvalidOperationException: The entity type 'BaseEvent' requires a primary key to be defined.
```

### Root Cause
Entity Framework Core was trying to map the `BaseEvent` abstract class as a database entity. This class is part of the Domain-Driven Design pattern for domain events and should not be persisted directly to the database.

### Solution
Updated `ApplicationDbContext.OnModelCreating` to:
1. Explicitly ignore the `BaseEvent` type
2. Configure all entities inheriting from `BaseEntity` to ignore the `DomainEvents` property

### Code Changes

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    // Ignore BaseEvent as it's not an entity
    builder.Ignore<BaseEvent>();

    // Configure BaseEntity
    foreach (var entityType in builder.Model.GetEntityTypes())
    {
        if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity)))
        {
            // Ignore the DomainEvents property
            builder.Entity(entityType.ClrType)
                .Ignore(nameof(BaseEntity.DomainEvents));
        }
    }

    base.OnModelCreating(builder);
}
```

## Additional Fixes

### 1. Application Insights Configuration
- Made Application Insights optional for local development
- Added conditional logic to only configure when connection string is present
- Prevents startup failures when running locally without Azure services

### 2. Development Configuration
- Created `appsettings.Development.json` with safe defaults
- Enabled in-memory database for development (no SQL Server required)
- Added dummy Azure AD B2C configuration to prevent errors

### 3. Database Initialization
- Created `ApplicationDbContextInitialiser` class
- Added automatic database creation and seeding for development
- Includes sample data for testing

## Running the Application

The application should now start successfully without external dependencies:

```bash
cd src/api/Emr.Api
dotnet run
```

Navigate to:
- http://localhost:5000 (redirects to Swagger)
- https://localhost:5001 (redirects to Swagger)

## Remaining Warnings

The build shows warnings but no errors:
- Microsoft.Identity.Web vulnerability warnings (package is at latest available version)
- FHIR client obsolete method warnings (can be addressed in future refactoring)

These warnings do not prevent the application from running.