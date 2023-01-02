param applicationInsightsName string
param location string

resource applicationInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: applicationInsightsName
  location: location
  tags: {
    displayName: 'Application Insights'
  }
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    RetentionInDays: 90
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

output applicationInsightsInstrumentationKeyOutput string = reference(applicationInsights.id, '2014-04-01').InstrumentationKey
