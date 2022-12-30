param storageAccountName string
param location string = resourceGroup().location

@allowed([
  'Standard_LRS'
  'Standard_ZRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Premium_LRS'
])
param storageAccountType string = 'Standard_LRS'

@allowed([
  'Standard'
])
param storageAccountTier string = 'Standard'
param resourceGroupName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2018-07-01' = {
  name: storageAccountName
  location: location
  tags: {
    displayName: 'Storage Account'
  }
  sku: {
    name: storageAccountType
    tier: storageAccountTier
  }
  kind: 'Storage'
  properties: {
    azureFilesAadIntegration: false
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        file: {
          enabled: true
        }
        blob: {
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }
}

output storageAccountConnectionString string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listKeys(resourceId(resourceGroupName, 'Microsoft.Storage/storageAccounts', storageAccountName), '2019-04-01').keys[0].value};EndpointSuffix=core.windows.net'
