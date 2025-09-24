# EMR System - Electronic Medical Records

A production-ready US EMR system built with .NET 8, React, React Native, and Azure infrastructure.

## Architecture Overview

- **Backend**: ASP.NET Core (.NET 8) with Clean Architecture
- **Frontend**: React 18 with TypeScript
- **Mobile**: React Native with Expo
- **Infrastructure**: Azure (App Service, SQL, Blob Storage, FHIR, B2C)
- **Interoperability**: FHIR R4 (US Core), HL7 v2 adapters

## Repository Structure

```
├── infra/              # Infrastructure as Code (Bicep/Terraform)
├── src/
│   ├── api/           # .NET Backend
│   │   ├── Emr.Domain/
│   │   ├── Emr.Application/
│   │   ├── Emr.Infrastructure/
│   │   └── Emr.Api/
│   ├── integration/   # Azure Functions
│   ├── workers/       # Background services
│   ├── web/          # React SPA
│   └── mobile/       # React Native app
├── docs/             # Documentation
└── tests/            # Test projects
```

## Features

### Core Functionality
- **Patient Portal**: View records, timeline, results, documents, consents
- **Hospital Portal**: QR patient linking, intake, document management
- **Mobile App**: QR scanning, consent management, record viewing

### Security & Compliance
- Azure AD B2C authentication
- Role-based access control (RBAC)
- Consent-based data sharing with QR codes
- PHI encryption and audit logging
- 42 CFR Part 2 compliance support

### Interoperability
- FHIR R4 integration (US Core profiles)
- HL7 v2 message processing
- Document/imaging management
- X12 eligibility (stub)

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Docker Desktop
- Azure CLI
- Visual Studio 2022 or VS Code

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd emr-system
   ```

2. **Backend Setup**
   ```bash
   cd src/api
   
   # Install Entity Framework tools
   dotnet tool install --global dotnet-ef
   
   # Set up local database
   dotnet ef database update -p Emr.Infrastructure -s Emr.Api
   
   # Run the API
   cd Emr.Api
   dotnet run
   ```

3. **Configure Secrets**
   ```bash
   # In src/api/Emr.Api directory
   dotnet user-secrets init
   
   # Set Azure AD B2C configuration
   dotnet user-secrets set "AzureAdB2C:Instance" "https://<tenant>.b2clogin.com"
   dotnet user-secrets set "AzureAdB2C:ClientId" "<client-id>"
   dotnet user-secrets set "AzureAdB2C:Domain" "<tenant>.onmicrosoft.com"
   
   # Set FHIR server configuration
   dotnet user-secrets set "FhirServer:Url" "https://<fhir-service>.fhir.azurehealthcareapis.com"
   dotnet user-secrets set "FhirServer:ClientId" "<client-id>"
   dotnet user-secrets set "FhirServer:ClientSecret" "<secret>"
   
   # Set Azure Storage
   dotnet user-secrets set "ConnectionStrings:AzureStorage" "<connection-string>"
   ```

4. **Web App Setup**
   ```bash
   cd src/web
   npm install
   
   # Create .env.local
   echo "REACT_APP_API_URL=https://localhost:5001" > .env.local
   echo "REACT_APP_B2C_CLIENT_ID=<client-id>" >> .env.local
   echo "REACT_APP_B2C_AUTHORITY=https://<tenant>.b2clogin.com/<tenant>.onmicrosoft.com/B2C_1_SignUpSignIn" >> .env.local
   
   npm start
   ```

5. **Mobile App Setup**
   ```bash
   cd src/mobile
   npm install
   
   # Create .env
   echo "API_URL=https://localhost:5001" > .env
   echo "B2C_CLIENT_ID=<client-id>" >> .env
   
   npm run start
   ```

### Using Docker

```bash
# Build and run all services
docker-compose up -d

# API will be available at http://localhost:5000
# Web app at http://localhost:3000
```

## Configuration

### Environment Variables

#### API (.NET)
- `ASPNETCORE_ENVIRONMENT`: Development/Staging/Production
- `ConnectionStrings__DefaultConnection`: SQL Server connection
- `ConnectionStrings__AzureStorage`: Blob storage connection
- `AzureAdB2C__*`: B2C configuration
- `FhirServer__*`: FHIR service configuration
- `ApplicationInsights__ConnectionString`: App Insights connection

#### Web App (React)
- `REACT_APP_API_URL`: Backend API URL
- `REACT_APP_B2C_CLIENT_ID`: B2C client ID
- `REACT_APP_B2C_AUTHORITY`: B2C authority URL

#### Mobile App (React Native)
- `API_URL`: Backend API URL
- `B2C_CLIENT_ID`: B2C client ID

## API Documentation

Once running, access Swagger UI at: `https://localhost:5001/index.html`

### Key Endpoints

- `POST /api/v1/connect/qr` - Generate QR code for patient linking
- `POST /api/v1/connect/qr/{code}/authorize` - Patient authorizes connection
- `GET /api/v1/connect/qr/{code}/exchange` - Exchange code for access token

## Security Notes

1. **Never commit secrets** - Use Azure Key Vault or user secrets
2. **Enable HTTPS** in production
3. **Configure CORS** appropriately for your domains
4. **Set up rate limiting** per environment
5. **Enable audit logging** for all PHI access

## Database Migrations

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> -p Emr.Infrastructure -s Emr.Api

# Update database
dotnet ef database update -p Emr.Infrastructure -s Emr.Api

# Generate SQL script
dotnet ef migrations script -p Emr.Infrastructure -s Emr.Api -o migration.sql
```

## Testing

```bash
# Run backend tests
dotnet test

# Run web app tests
cd src/web && npm test

# Run E2E tests
cd tests/web && npm run test:e2e
```

## Deployment

### Azure Deployment

1. **Infrastructure**
   ```bash
   cd infra
   az bicep build --file main.bicep
   az deployment sub create --location eastus --template-file main.bicep --parameters @parameters.dev.json
   ```

2. **Backend**
   - Deployed via GitHub Actions to Azure App Service
   - Migrations run automatically on deployment

3. **Frontend**
   - Web app deployed to Azure Static Web Apps
   - Mobile app deployed via Expo EAS

## Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check connection string in appsettings.json
   - Ensure SQL Server is running
   - Verify firewall rules

2. **FHIR Connection Failed**
   - Verify FHIR service URL
   - Check authentication configuration
   - Ensure service principal has proper permissions

3. **B2C Authentication Issues**
   - Verify redirect URIs in B2C app registration
   - Check token validation settings
   - Ensure proper scopes are configured

## Contributing

1. Create a feature branch
2. Make changes following coding standards
3. Add tests for new functionality
4. Submit PR with detailed description

## License

[License Type] - See LICENSE file for details