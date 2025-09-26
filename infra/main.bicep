targetScope = 'subscription'

@description('The environment name (dev, test, prod)')
param environment string = 'dev'

@description('The Azure region for resources')
param location string = 'eastus'

@description('The organization name')
param organizationName string = 'emr'

@description('The resource group name')
param resourceGroupName string = 'rg-${organizationName}-${environment}'

@description('Tags to apply to all resources')
param tags object = {
  Environment: environment
  Project: 'EMR System'
  ManagedBy: 'Bicep'
}

// Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

// Deploy resources into the resource group
module resources './resources.bicep' = {
  name: 'resources-deployment'
  scope: rg
  params: {
    environment: environment
    location: location
    organizationName: organizationName
    tags: tags
  }
}

// Outputs
output resourceGroupName string = rg.name
output resourceGroupId string = rg.id