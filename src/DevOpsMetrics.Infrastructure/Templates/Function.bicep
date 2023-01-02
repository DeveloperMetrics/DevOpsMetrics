param webSiteName string
param hostingPlanName string
param applicationInsightsInstrumentationKey string
param webServiceURL string
param keyVaultName string
param storageConnectionString string
param azureDevOpsPatToken string
param gitHubClientId string
@secure()
param gitHubClientSecret string
param location string

resource webSite 'Microsoft.Web/sites@2018-11-01' = {
  name: webSiteName
  location: location
  kind: 'functionapp'
  tags: {
    displayName: 'Function'
  }
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', hostingPlanName)
    httpsOnly: true
    siteConfig:{
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsightsInstrumentationKey
        }
        {
          name: 'AppSettings:WebServiceURL'
          value: webServiceURL
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
        {
          name: 'AzureStorageAccountContainerAzureDevOpsBuilds'
          value: 'DevOpsAzureDevOpsBuilds'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerAzureDevOpsPRs'
          value: 'DevOpsAzureDevOpsPRs'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits'
          value: 'DevOpsAzureDevOpsPRCommits'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerAzureDevOpsSettings'
          value: 'DevOpsAzureDevOpsSettings'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerGitHubRuns'
          value: 'DevOpsGitHubRuns'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerTableLog'
          value: 'DevOpsLog'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerChangeFailureRate'
          value: 'DevOpsChangeFailureRate'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerMTTR'
          value: 'DevOpsMTTR'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerGitHubSettings'
          value: 'DevOpsGitHubSettings'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerGitHubPRCommits'
          value: 'DevOpsGitHubPRCommits'
        }
        {
          name: 'AppSettings:AzureStorageAccountContainerGitHubPRs'
          value: 'DevOpsGitHubPRs'
        }
      ]
    }
  }
  dependsOn: []
}
