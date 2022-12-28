param webSiteName string
param hostingPlanName string
param applicationInsightsInstrumentationKey string
param keyVaultName string = ''

resource webSite 'Microsoft.Web/sites@2018-11-01' = {
  name: webSiteName
  location: resourceGroup().location
  kind: 'app'
  tags: {
    displayName: 'Web Service Webapp'
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
      ]
    }
  }
  dependsOn: []
}
