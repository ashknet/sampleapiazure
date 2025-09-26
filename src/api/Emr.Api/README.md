# EMR API - Running Instructions

## Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (or configure to use in-memory database)
- Visual Studio 2022 or VS Code with C# extensions

## Configuration

### Development Environment
The application is configured to run with minimal external dependencies in development mode:

1. **Application Insights**: Optional in development. If not configured, logs will only go to console.
2. **Azure AD B2C**: Uses dummy values in development. Real authentication is bypassed.
3. **Database**: Uses in-memory database by default in development (no SQL Server required).
4. **FHIR Server**: Points to localhost. Can be mocked or stubbed for testing.

### Running the Application

1. **From Visual Studio**:
   - Open the solution file `EmrSystem.sln`
   - Set `Emr.Api` as the startup project
   - Press F5 or click the Run button
   - The browser will open to Swagger UI at https://localhost:5001/swagger

2. **From Command Line**:
   ```bash
   cd src/api/Emr.Api
   dotnet run
   ```
   - Navigate to http://localhost:5000/swagger or https://localhost:5001/swagger

3. **Using Docker** (if Docker is installed):
   ```bash
   docker-compose up
   ```

## Troubleshooting

### Application Insights Error
If you see an error about TelemetryConfiguration:
- This is expected if Application Insights is not configured
- The application will still run, logging only to console
- To enable Application Insights, add a valid connection string in appsettings.json

### Authentication Errors
- In development, authentication is mocked
- You can test endpoints without real Azure AD B2C credentials
- For production, configure real Azure AD B2C settings

### Database Connection Errors
- By default, uses in-memory database in development
- To use SQL Server, set `UseInMemoryDatabase` to `false` in appsettings.Development.json
- Ensure LocalDB is installed or update the connection string

## Environment Variables
You can override settings using environment variables:
- `ASPNETCORE_ENVIRONMENT`: Set to "Development" for local development
- `ApplicationInsights__ConnectionString`: Application Insights connection string
- `ConnectionStrings__DefaultConnection`: Database connection string

## Health Check
Check if the API is running:
```
GET http://localhost:5000/health
```

## Default Seed Data
When running in development with in-memory database, the following test data is created:
- Organizations: General Hospital, City Medical Clinic
- Users: test-patient-001, test-clinician-001, test-registrar-001
- Roles: Patient, Clinician, Registrar, HIM, OrgAdmin, Integration, Auditor
- Sample locations, departments, payers, and plans

## API Documentation
Once running, navigate to the Swagger UI for interactive API documentation:
- http://localhost:5000 (redirects to Swagger)
- https://localhost:5001 (redirects to Swagger)