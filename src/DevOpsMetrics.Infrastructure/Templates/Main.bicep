// To lint this file: az bicep build --file main.bicep
// To validate this file: az deployment sub validate --template-file main.bicep --location "eastus" --parameters resourcesSuffix="rm4"
// To preview this file: az deployment sub what-if --template-file main.bicep --location "eastus" --parameters resourcesSuffix="rm4"
// To deploy this file: az deployment sub create --template-file main.bicep --location "eastus" --parameters resourcesSuffix="rm4"


targetScope = 'subscription'

param location string = deployment().location
param resourcesSuffix string = ''
param azureDevOpsPatToken string = ''
param gitHubClientId string = ''
@secure()
param gitHubClientSecret string = ''


var resourceGroupName = 'rg-devopsmetrics-${resourcesSuffix}'
var managedIdentityName='app-id-devops-${resourcesSuffix}'
var keyVaultName='vault-devops-prd-eu-${resourcesSuffix}'
var storageName='stgdevopsprdeu${resourcesSuffix}'
var hostingName='hosting-devops-prd-eu-${resourcesSuffix}'
var appInsightsName='appinsights-devops-prd-eu-${resourcesSuffix}'
var serviceName='service-devops-prd-eu-${resourcesSuffix}'
var websiteName='web-devops-prd-eu-${resourcesSuffix}'
var functionName='function-devops-prd-eu-${resourcesSuffix}'

module devopsResourceGroup './ResourceGroup.bicep' = {
  name: '${resourceGroupName}-resourceGroupDeployment'
  params: {
    resourceGroupName: resourceGroupName
    location: location
  }
}

module managedIdentity './ManagedIdentity.bicep' = {
  name: '${managedIdentityName}-Deployment'
  params: {
    location: location
    name: managedIdentityName
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    devopsResourceGroup
  ]
}

module keyVault './KeyVault.bicep' = {
  name: '${keyVaultName}-Deployment'
  params: {
    location: location
    keyVaultName: keyVaultName
    administratorUserPrincipalId: managedIdentity.outputs.userAssignedManagedIdentityPrincipalId
    storageAccountConnectionString: storage.outputs.storageAccountConnectionString
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    managedIdentity
  ]
}

module storage './Storage.bicep' = {
  name: '${storageName}-Deployment'
  params: {
    location: location
    storageAccountName: storageName
    resourceGroupName: resourceGroupName
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    devopsResourceGroup
  ]
}

module webHosting './WebHosting.bicep' = {
  name: '${hostingName}-Deployment'
  params: {
    location: location
    hostingPlanName: hostingName
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    devopsResourceGroup
  ]
}

module appInsights './ApplicationInsights.bicep' = {
  name: '${appInsightsName}-Deployment'
  params: {
    location: location
    applicationInsightsName: appInsightsName
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    devopsResourceGroup
  ]
}

module webService './Website.bicep' = {
  name: '${serviceName}-Deployment'
  params: {
    location: location
    webSiteName: serviceName 
    hostingPlanName: hostingName
    managedIdentityId: managedIdentity.outputs.userAssignedManagedIdentityId
    managedIdentityClientId: managedIdentity.outputs.userAssignedManagedIdentityClientId
    applicationInsightsInstrumentationKey:appInsights.outputs.applicationInsightsInstrumentationKeyOutput
    storageConnectionString: storage.outputs.storageAccountConnectionString
    keyVaultName:keyVaultName
    azureDevOpsPatToken: azureDevOpsPatToken
    gitHubClientId: gitHubClientId
    gitHubClientSecret: gitHubClientSecret
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    appInsights
    storage
  ]
}

module webSite './Website.bicep' = {
  name: '${websiteName}-Deployment'
  params: {
    location: location
    webSiteName: websiteName 
    hostingPlanName: hostingName
    managedIdentityId: managedIdentity.outputs.userAssignedManagedIdentityId
    applicationInsightsInstrumentationKey:appInsights.outputs.applicationInsightsInstrumentationKeyOutput
    webServiceURL: webService.outputs.url
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    appInsights
    storage
    webService
  ]
}

module function './Function.bicep' = {
  name: '${functionName}-Deployment'
  params: {
    location: location
    webSiteName: functionName 
    hostingPlanName: hostingName
    applicationInsightsInstrumentationKey:appInsights.outputs.applicationInsightsInstrumentationKeyOutput
    storageConnectionString: storage.outputs.storageAccountConnectionString
    keyVaultName:keyVaultName
    azureDevOpsPatToken: azureDevOpsPatToken
    gitHubClientId: gitHubClientId
    gitHubClientSecret: gitHubClientSecret
    webServiceURL: webService.outputs.url
  }
  scope: resourceGroup(resourceGroupName)
  dependsOn: [
    appInsights
    storage
    webService
  ]
}


