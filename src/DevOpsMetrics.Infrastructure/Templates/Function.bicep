param webSiteName string
param hostingPlanName string
param applicationInsightsInstrumentationKey string

resource webSite 'Microsoft.Web/sites@2018-11-01' = {
  name: webSiteName
  location: resourceGroup().location
  kind: 'functionapp'
  tags: {
    displayName: 'Function'
  }
  properties: {
    name: webSiteName
    serverFarmId: resourceId('Microsoft.Web/serverfarms', hostingPlanName)
    httpsOnly: true
    siteConfig:{
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsightsInstrumentationKey
        }
      ]
    }
  }
  dependsOn: []
}
