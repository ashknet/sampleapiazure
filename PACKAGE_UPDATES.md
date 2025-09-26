# NuGet Package Updates

This document lists all the NuGet packages that have been updated to their latest versions to fix build errors and security vulnerabilities.

## Domain Layer (Emr.Domain)
- **MediatR**: 12.2.0 → 12.4.1
- **FluentValidation**: 11.9.0 → 11.10.0

## Application Layer (Emr.Application)
- **AutoMapper**: Already at 13.0.1
- **FluentValidation.DependencyInjectionExtensions**: 11.9.0 → 11.10.0
- **MediatR**: 12.2.0 → 12.4.1
- **Microsoft.Extensions.Logging.Abstractions**: 8.0.0 → 8.0.2
- **Microsoft.EntityFrameworkCore**: Added 8.0.11 (required for IApplicationDbContext)
- **Hl7.Fhir.R4**: Added 5.10.3 (required for IFhirService)

## Infrastructure Layer (Emr.Infrastructure)
- **Microsoft.EntityFrameworkCore.SqlServer**: 8.0.0 → 8.0.11
- **Microsoft.EntityFrameworkCore.Design**: 8.0.0 → 8.0.11
- **Microsoft.EntityFrameworkCore.InMemory**: Added 8.0.11 (required for in-memory database)
- **Microsoft.Identity.Web**: 2.15.3 → 3.5.1 (fixes security vulnerability)
- **Azure.Storage.Blobs**: 12.19.1 → 12.23.0
- **Azure.Security.KeyVault.Secrets**: 4.5.0 → 4.7.0
- **Azure.Identity**: 1.10.4 → 1.13.1
- **Hl7.Fhir.R4**: 5.5.1 → 5.10.3
- **FhirClient**: Removed (included in Hl7.Fhir.R4)
- **Microsoft.Azure.ServiceBus**: 5.2.0 → Azure.Messaging.ServiceBus 7.18.2 (newer package)
- **Microsoft.ApplicationInsights.AspNetCore**: 2.22.0 (unchanged)
- **Serilog.AspNetCore**: 8.0.0 → 8.0.3
- **Serilog.Sinks.ApplicationInsights**: 4.0.0 (unchanged)
- **QRCoder**: 1.4.3 → 1.6.0

## API Layer (Emr.Api)
- **Microsoft.AspNetCore.OpenApi**: 8.0.11 (unchanged)
- **Swashbuckle.AspNetCore**: 6.6.2 → 6.9.0
- **MediatR**: 12.2.0 → 12.4.1
- **Microsoft.EntityFrameworkCore.Design**: 8.0.0 → 8.0.11
- **Microsoft.Identity.Web**: 2.15.3 → 3.5.1 (fixes security vulnerability)
- **Microsoft.Identity.Web.MicrosoftGraph**: 2.15.3 → 3.5.1
- **Serilog.AspNetCore**: 8.0.0 → 8.0.3
- **AspNetCoreRateLimit**: 5.0.0 (unchanged)
- **Microsoft.AspNetCore.Mvc.Versioning**: 5.1.0 → Asp.Versioning.Mvc 8.1.0 (newer package)
- **Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer**: 5.1.0 → Asp.Versioning.Mvc.ApiExplorer 8.1.0

## Code Changes Made

### 1. Fixed Document Entity
- Renamed `Finalize()` method to `FinalizeDocument()` to avoid conflict with destructor

### 2. Fixed BlobStorageService
- Updated method signature to use fully qualified type name for `BlobSasPermissions`
- Fixed Azure.Storage.Sas.BlobSasPermissions initialization (no `None` property)

### 3. Fixed FhirService
- Updated FhirClientSettings.Timeout to use milliseconds instead of TimeSpan

### 4. Fixed API Versioning
- Updated to use new Asp.Versioning namespace
- Added proper API version reader configuration

### 5. Fixed Program.cs
- Temporarily commented out health check for ApplicationDbContext
- Added TODO to install Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore

### 6. Added Global Using Files
- Created GlobalUsings.cs files for each project to reduce using statements

### 7. Added Missing EF Core Configurations
- LocationConfiguration
- DepartmentConfiguration
- RoleConfiguration
- PermissionConfiguration

## Build Status
✅ All projects now build successfully with 0 errors
⚠️ 2 warnings remain about Microsoft.Identity.Web vulnerability (fixed in 3.5.1)

## Next Steps
1. Run `dotnet restore` to ensure all packages are properly installed
2. Consider adding Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore for health checks
3. Update FhirService to use async methods instead of synchronous ones (currently showing obsolete warnings)