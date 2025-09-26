# EMR Database Setup

This folder contains the SQL Server database schema for the EMR system.

## Prerequisites

- SQL Server 2019 or later (or SQL Server LocalDB for development)
- SQL Server Management Studio (SSMS) or Azure Data Studio
- PowerShell (for automated deployment)

## Database Structure

The database includes the following main tables:

### Core Tables
- **Organizations** - Healthcare organizations/hospitals
- **Locations** - Physical locations within organizations
- **Departments** - Departments within locations
- **Users** - System users (patients, staff, clinicians)
- **Roles** - User roles (Patient, Clinician, Registrar, HIM, OrgAdmin)
- **Permissions** - Granular permissions for role-based access

### Patient Management
- **Patients** - Patient demographic information
- **PatientIdentifiers** - SSN, driver's license, etc.
- **PatientContacts** - Emergency contacts
- **PatientAddresses** - Patient addresses

### Insurance & Coverage
- **Payers** - Insurance companies
- **Plans** - Insurance plans
- **Coverages** - Patient insurance coverage

### Consent & Sharing
- **Consents** - Patient consent records
- **ConsentEvents** - Consent audit trail
- **ShareTokens** - Tokens for sharing data
- **ConnectionLinks** - QR code links for consent

### Clinical Data
- **Documents** - Clinical documents
- **DocumentShares** - Document sharing records
- **Appointments** - Patient appointments

### System
- **AuditLogs** - Comprehensive audit trail
- **IntegrationEndpoints** - External system integrations
- **NotificationPreferences** - User notification settings

## Deployment Options

### Option 1: Using SQL Server Management Studio (SSMS)

1. Open SSMS and connect to your SQL Server instance
2. Open `Deploy-Database.sql`
3. Update the database name if needed (default: EmrDb_Dev)
4. Execute the script (F5)

### Option 2: Using PowerShell (Automated)

```powershell
# From the database folder
.\Deploy-Database.ps1

# With custom parameters
.\Deploy-Database.ps1 -ServerInstance "localhost\SQLEXPRESS" -DatabaseName "EmrDb" -DropExisting

# Parameters:
# -ServerInstance: SQL Server instance (default: (localdb)\mssqllocaldb)
# -DatabaseName: Database name (default: EmrDb_Dev)
# -DropExisting: Drop and recreate database if it exists
```

### Option 3: Manual Execution

1. Create the database:
   ```sql
   CREATE DATABASE EmrDb_Dev;
   ```

2. Run the scripts in order:
   - `dbo/Scripts/CreateAllTables.sql`
   - `dbo/Scripts/PostDeployment.sql`

## Connection Strings

### LocalDB (Development)
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmrDb_Dev;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### SQL Server Express
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EmrDb_Dev;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### SQL Server with SQL Authentication
```json
"DefaultConnection": "Server=localhost;Database=EmrDb_Dev;User Id=sa;Password=YourPassword;MultipleActiveResultSets=true"
```

### Azure SQL Database
```json
"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=EmrDb;User ID=yourusername;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;"
```

## Entity Framework Migrations

After creating the database, you can use Entity Framework migrations:

```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Navigate to API project
cd ../../api/Emr.Api

# Create a new migration
dotnet ef migrations add InitialCreate --project ../Emr.Infrastructure/Emr.Infrastructure.csproj

# Apply migrations to database
dotnet ef database update
```

## Development Tips

1. **Use LocalDB for Development**: LocalDB is lightweight and doesn't require a full SQL Server installation
2. **Enable Query Store**: For performance monitoring in production
3. **Regular Backups**: Set up automated backups for production
4. **Index Maintenance**: Schedule regular index maintenance jobs

## Security Considerations

1. **Encryption**: Enable Transparent Data Encryption (TDE) for production
2. **Row-Level Security**: Consider implementing RLS for multi-tenant scenarios
3. **Always Encrypted**: For sensitive fields like SSN
4. **Audit Logs**: The AuditLogs table tracks all data access

## Troubleshooting

### Cannot connect to LocalDB
```powershell
# Start LocalDB
sqllocaldb start MSSQLLocalDB

# Check LocalDB instances
sqllocaldb info
```

### Permission Issues
Ensure your Windows user has db_owner permissions on the database.

### Migration Conflicts
If EF migrations conflict with manual schema changes:
1. Generate a SQL script: `dotnet ef migrations script`
2. Review and manually apply necessary changes

## Schema Conventions

- **Primary Keys**: All tables use UNIQUEIDENTIFIER (GUID) primary keys
- **Soft Deletes**: IsDeleted flag with DeletedAt/DeletedBy tracking
- **Audit Fields**: CreatedAt/By, UpdatedAt/By on all tables
- **Indexes**: Unique constraints respect soft deletes (WHERE IsDeleted = 0)

## Next Steps

1. Deploy the database using one of the methods above
2. Update connection string in `appsettings.Development.json`
3. Run the API project to verify connectivity
4. Seed additional test data as needed