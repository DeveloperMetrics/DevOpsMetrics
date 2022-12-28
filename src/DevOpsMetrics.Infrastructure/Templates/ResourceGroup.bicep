targetScope = 'subscription'

param resourceGroupName string
param location string

// https://docs.microsoft.com/en-us/azure/templates/microsoft.resources/resourcegroups?tabs=bicep
resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}
