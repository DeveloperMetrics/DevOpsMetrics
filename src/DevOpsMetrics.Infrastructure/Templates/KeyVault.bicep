param keyVaultName string
param storageAccountConnectionString string

@description('Permissions to grant admin secrets in the vault. (Valid values are: all, get, set, list, and delete.)')
param vaultAdminSecretsPermissions array = [
  'all'
]

@description('Permissions to grant user access to get and list secrets in the vault.')
param vaultUserSecretsPermissions array = [
  'get'
  'list'
]

@description('SKU for the vault')
@allowed([
  'Standard'
  'Premium'
])
param vaultSku string = 'Standard'

@description('Specifies if the vault is enabled for VM or Service Fabric deployment')
param vaultEnabledForDeployment bool = false

@description('Specifies if the vault is enabled for ARM template deployment')
param vaultEnabledForTemplateDeployment bool = true

@description('Object Id of the AAD user that will have admin access to the SQL server and the Key Vault. Available from the Get-AzureRMADUser or the Get-AzureRMADServicePrincipal cmdlets')
param administratorUserPrincipalId string

resource keyVault 'Microsoft.KeyVault/vaults@2016-10-01' = {
  name: keyVaultName
  location: resourceGroup().location
  tags: {
    displayName: 'KeyVault'
  }
  properties: {
    enabledForDeployment: vaultEnabledForDeployment
    enabledForTemplateDeployment: vaultEnabledForTemplateDeployment
    enableSoftDelete: true
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: vaultSku
    }
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: administratorUserPrincipalId
        permissions: {
          secrets: vaultAdminSecretsPermissions
        }
      }
    ]
  }
}

resource storageConnString 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  parent: keyVault
  name: 'AppSettings--AzureStorageAccountConfigurationString'
  
  properties: {
    value: storageAccountConnectionString
  }
}
