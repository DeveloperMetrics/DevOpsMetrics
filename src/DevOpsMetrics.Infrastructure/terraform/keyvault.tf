resource "azurerm_key_vault" "az_key_vault" {
  #checkov:skip=CKV_AZURE_109: "Ensure key vault allows firewall rules settings"
  #checkov:skip=CKV_AZURE_42: "Ensure the key vault is recoverable"
  #checkov:skip=CKV_AZURE_110: "Ensure that key vault enables purge protection"
  name                        = local.keyvaultname
  location                    = azurerm_resource_group.application.location
  resource_group_name         = azurerm_resource_group.application.name
  enabled_for_disk_encryption = "false"
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 90
  purge_protection_enabled    = "false"
  sku_name                    = "standard"
}

resource "azurerm_key_vault_access_policy" "kv_access_policy" {
  key_vault_id = azurerm_key_vault.az_key_vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id
  key_permissions = [
    "Get",
    "List"
  ]
  secret_permissions = [
    "Get",
    "List",
    "Set",
    "Delete",
    "Purge",
    "Recover"
  ]
  certificate_permissions = [
    "Get",
    "List"
  ]
}