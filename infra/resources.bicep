targetScope = 'resourceGroup'

@description('The environment name (dev, test, prod)')
param environment string

@description('The Azure region for resources')
param location string

@description('The organization name')
param organizationName string

@description('Tags to apply to all resources')
param tags object

@description('SQL Server administrator login')
@secure()
param sqlAdminLogin string = 'emradmin'

@description('SQL Server administrator password')
@secure()
param sqlAdminPassword string

// Variables
var uniqueSuffix = uniqueString(resourceGroup().id)
var appName = '${organizationName}-${environment}'
var keyVaultName = 'kv-${appName}-${uniqueSuffix}'
var storageAccountName = 'st${organizationName}${environment}${uniqueSuffix}'
var appServicePlanName = 'asp-${appName}'
var apiAppServiceName = 'app-${appName}-api'
var sqlServerName = 'sql-${appName}-${uniqueSuffix}'
var sqlDatabaseName = 'sqldb-${appName}'
var serviceBusNamespaceName = 'sb-${appName}-${uniqueSuffix}'
var appInsightsName = 'appi-${appName}'
var logAnalyticsName = 'log-${appName}'
var apimName = 'apim-${appName}-${uniqueSuffix}'
var b2cTenantName = '${organizationName}${environment}'
var healthDataServicesWorkspaceName = 'hdw-${appName}-${uniqueSuffix}'
var fhirServiceName = 'fhir-${appName}'
var dicomServiceName = 'dicom-${appName}'

// Log Analytics Workspace
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: logAnalyticsName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
  }
}

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
}

// Blob containers
var blobContainers = [
  'documents'
  'images'
  'temp'
  'audit'
]

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2021-06-01' = {
  parent: storageAccount
  name: 'default'
}

resource containers 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = [for container in blobContainers: {
  parent: blobService
  name: container
  properties: {
    publicAccess: 'None'
  }
}]

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2021-05-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-05-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  tags: tags
  sku: {
    name: environment == 'prod' ? 'S3' : 'Basic'
    tier: environment == 'prod' ? 'Standard' : 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: environment == 'prod' ? 268435456000 : 2147483648
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
  }
}

// SQL Firewall Rule for Azure Services
resource sqlFirewallRule 'Microsoft.Sql/servers/firewallRules@2021-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: environment == 'prod' ? 'P1v3' : 'B1'
    tier: environment == 'prod' ? 'PremiumV3' : 'Basic'
    size: environment == 'prod' ? 'P1v3' : 'B1'
    family: environment == 'prod' ? 'Pv3' : 'B'
    capacity: environment == 'prod' ? 2 : 1
  }
  properties: {
    reserved: false // Windows
  }
}

// API App Service
resource apiAppService 'Microsoft.Web/sites@2021-02-01' = {
  name: apiAppServiceName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prod' ? 'Production' : 'Development'
        }
        {
          name: 'ApplicationInsights:InstrumentationKey'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'KeyVault:Uri'
          value: keyVault.properties.vaultUri
        }
      ]
      connectionStrings: [
        {
          name: 'DefaultConnection'
          connectionString: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
          type: 'SQLAzure'
        }
      ]
    }
  }
}

// Service Bus Namespace
resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: serviceBusNamespaceName
  location: location
  tags: tags
  sku: {
    name: environment == 'prod' ? 'Premium' : 'Standard'
    tier: environment == 'prod' ? 'Premium' : 'Standard'
    capacity: environment == 'prod' ? 1 : 0
  }
  properties: {
    zoneRedundant: environment == 'prod' ? true : false
  }
}

// Service Bus Queues
var queues = [
  'document-processing'
  'notifications'
  'hl7-messages'
  'fhir-sync'
]

resource serviceBusQueues 'Microsoft.ServiceBus/namespaces/queues@2021-11-01' = [for queue in queues: {
  parent: serviceBusNamespace
  name: queue
  properties: {
    lockDuration: 'PT1M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    defaultMessageTimeToLive: 'P7D'
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 10
  }
}]

// Azure Health Data Services Workspace
resource healthDataServicesWorkspace 'Microsoft.HealthcareApis/workspaces@2022-06-01' = {
  name: healthDataServicesWorkspaceName
  location: location
  tags: tags
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

// FHIR Service
resource fhirService 'Microsoft.HealthcareApis/workspaces/fhirservices@2022-06-01' = {
  parent: healthDataServicesWorkspace
  name: fhirServiceName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    authenticationConfiguration: {
      authority: uri(az.environment().authentication.loginEndpoint, subscription().tenantId)
      audience: 'https://${healthDataServicesWorkspaceName}-${fhirServiceName}.fhir.azurehealthcareapis.com'
      smartProxyEnabled: false
    }
    corsConfiguration: {
      origins: ['*']
      headers: ['*']
      methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS']
      allowCredentials: false
    }
  }
}

// DICOM Service
resource dicomService 'Microsoft.HealthcareApis/workspaces/dicomservices@2022-06-01' = {
  parent: healthDataServicesWorkspace
  name: dicomServiceName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    corsConfiguration: {
      origins: ['*']
      headers: ['*']
      methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS']
      allowCredentials: false
    }
  }
}

// API Management (optional for production)
resource apim 'Microsoft.ApiManagement/service@2021-08-01' = if (environment == 'prod') {
  name: apimName
  location: location
  tags: tags
  sku: {
    name: 'Developer'
    capacity: 1
  }
  properties: {
    publisherEmail: 'admin@${organizationName}.com'
    publisherName: organizationName
  }
}

// Static Web App for React
resource staticWebApp 'Microsoft.Web/staticSites@2021-03-01' = {
  name: 'stapp-${appName}'
  location: 'eastus2' // Static Web Apps have limited region support
  tags: tags
  sku: {
    name: environment == 'prod' ? 'Standard' : 'Free'
    tier: environment == 'prod' ? 'Standard' : 'Free'
  }
  properties: {
    repositoryUrl: 'https://github.com/${organizationName}/emr-system'
    branch: environment == 'prod' ? 'main' : 'develop'
    buildProperties: {
      appLocation: '/src/web'
      apiLocation: ''
      outputLocation: 'dist'
    }
  }
}

// Outputs
output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
output storageAccountName string = storageAccount.name
output storageAccountConnectionString string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name
output sqlConnectionString string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
output apiAppServiceName string = apiAppService.name
output apiAppServiceUrl string = 'https://${apiAppService.properties.defaultHostName}'
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output serviceBusNamespaceName string = serviceBusNamespace.name
output serviceBusConnectionString string = serviceBusNamespace.listKeys('RootManageSharedAccessKey').primaryConnectionString
output fhirServiceUrl string = 'https://${healthDataServicesWorkspaceName}-${fhirServiceName}.fhir.azurehealthcareapis.com'
output dicomServiceUrl string = 'https://${healthDataServicesWorkspaceName}-${dicomServiceName}.dicom.azurehealthcareapis.com'
output staticWebAppUrl string = staticWebApp.properties.defaultHostname