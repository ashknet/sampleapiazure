#!/bin/bash

# Deploy EMR System Infrastructure
# Usage: ./deploy.sh <environment> <subscription-id>

set -e

# Parameters
ENVIRONMENT=${1:-dev}
SUBSCRIPTION_ID=${2}
LOCATION="eastus"
DEPLOYMENT_NAME="emr-${ENVIRONMENT}-$(date +%Y%m%d%H%M%S)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Functions
print_error() {
    echo -e "${RED}ERROR: $1${NC}"
}

print_success() {
    echo -e "${GREEN}SUCCESS: $1${NC}"
}

print_info() {
    echo -e "${YELLOW}INFO: $1${NC}"
}

# Validate parameters
if [ -z "$SUBSCRIPTION_ID" ]; then
    print_error "Subscription ID is required"
    echo "Usage: ./deploy.sh <environment> <subscription-id>"
    exit 1
fi

if [[ ! "$ENVIRONMENT" =~ ^(dev|test|prod)$ ]]; then
    print_error "Environment must be dev, test, or prod"
    exit 1
fi

print_info "Starting deployment for environment: $ENVIRONMENT"

# Login to Azure (if not already logged in)
if ! az account show &>/dev/null; then
    print_info "Please login to Azure..."
    az login
fi

# Set subscription
print_info "Setting subscription to: $SUBSCRIPTION_ID"
az account set --subscription "$SUBSCRIPTION_ID"

# Validate Bicep files
print_info "Validating Bicep templates..."
az bicep build --file main.bicep

# Check if SQL password exists in environment or prompt
if [ -z "$SQL_ADMIN_PASSWORD" ]; then
    read -s -p "Enter SQL Admin Password: " SQL_ADMIN_PASSWORD
    echo
fi

# Create deployment
print_info "Creating deployment: $DEPLOYMENT_NAME"
az deployment sub create \
    --name "$DEPLOYMENT_NAME" \
    --location "$LOCATION" \
    --template-file main.bicep \
    --parameters "parameters.${ENVIRONMENT}.json" \
    --parameters sqlAdminPassword="$SQL_ADMIN_PASSWORD" \
    --verbose

# Check deployment status
DEPLOYMENT_STATUS=$(az deployment sub show --name "$DEPLOYMENT_NAME" --query "properties.provisioningState" -o tsv)

if [ "$DEPLOYMENT_STATUS" == "Succeeded" ]; then
    print_success "Deployment completed successfully!"
    
    # Get outputs
    print_info "Deployment outputs:"
    az deployment sub show \
        --name "$DEPLOYMENT_NAME" \
        --query "properties.outputs" \
        -o json
else
    print_error "Deployment failed with status: $DEPLOYMENT_STATUS"
    exit 1
fi

print_info "Deployment complete!"