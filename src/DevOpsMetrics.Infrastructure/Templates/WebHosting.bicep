param hostingPlanName string
param location string = resourceGroup().location

@description('Describes plan\'s pricing tier and instance size. Check details at https://azure.microsoft.com/en-us/pricing/details/app-service/')
@allowed([
  'D1'
  'B1'
  'S1'
  'S2'
  'S3'
  'P1'
  'P2'
  'P3'
  'P4'
])
param hostingPlanSKUName string = 'B1'

@description('Describes plan\'s instance count')
@minValue(1)
param hostingPlanSKUCapacity int = 1

resource hostingPlan 'Microsoft.Web/serverfarms@2015-08-01' = {
  name: hostingPlanName
  location: location
  tags: {
    displayName: 'Hosting Plan'
  }
  sku: {
    name: hostingPlanSKUName
    capacity: hostingPlanSKUCapacity
  }
  properties: {
    name: hostingPlanName
  }
}
