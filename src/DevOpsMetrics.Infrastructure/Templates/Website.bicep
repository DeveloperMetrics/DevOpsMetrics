param webSiteName string
param hostingPlanName string
param managedIdentityId string
param managedIdentityClientId string
param applicationInsightsInstrumentationKey string
param storageConnectionString string = ''
param keyVaultName string = ''
@secure()
param azureDevOpsPatToken string = ''
param gitHubClientId string = ''
@secure()
param gitHubClientSecret string = ''
param webServiceURL string = ''
param location string 

resource webSite 'Microsoft.Web/sites@2018-11-01' = {
  name: webSiteName
  location: location
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
          name: 'AppSettings:AzureDevOpsPatToken'
          value: azureDevOpsPatToken
        }
        {
          name: 'AppSettings:GitHubClientId'
          value: gitHubClientId
        }
        {
          name: 'AppSettings:GitHubClientSecret'
          value: gitHubClientSecret
        }
        //This is set for debugging purposes - this will enable Swagger for the Web Service and more logging
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        {
          name: 'AppSettings:WebServiceURL'
          value: webServiceURL
        }
        {
          name: 'AppSettings:UseCache'
          value: 'false'
        }
        {
          // This is necessary for authenticating to Secret Vault: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#specifying-a-user-assigned-managed-identity-with-the-defaultazurecredential
          name: 'AZURE_CLIENT_ID'
          value: managedIdentityClientId
        }
      ]
    }
  }
  dependsOn: []
}

output url string = 'https://${webSite.properties.defaultHostName}'
