param webSiteName string
param hostingPlanName string
param managedIdentityId string
param applicationInsightsInstrumentationKey string
param storageConnectionString string = ''
param keyVaultName string = ''
// These to access Key Vault should not be here ideally - to be checked later
param keyVaultClientId string = ''
@secure()
param keyVaultSecret string = ''
param tenantId string = ''


resource webSite 'Microsoft.Web/sites@2018-11-01' = {
  name: webSiteName
  location: resourceGroup().location
  kind: 'app'
  tags: {
    displayName: 'Web Service Webapp'
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    name: webSiteName
    serverFarmId: resourceId('Microsoft.Web/serverfarms', hostingPlanName)
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsightsInstrumentationKey
        }
        {
          name: 'AppSettings:KeyVaultURL'
          value: 'https://${keyVaultName}.vault.azure.net/'
        }
        {
          name: 'AppSettings:AzureStorageAccountConfigurationString'
          value: storageConnectionString
        }
        {
          name: 'AppSettings:KeyVaultClientId'
          value: keyVaultClientId
        }
        {
          name: 'AppSettings:KeyVaultClientSecret'
          value: keyVaultSecret
        }
        {
          name: 'AppSettings:TenantId'
          value: tenantId
        }
      ]
    }
  }
  dependsOn: []
}
