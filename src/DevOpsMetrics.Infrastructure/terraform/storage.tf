resource "azurerm_storage_account" "storage" {
  #checkov:skip=CKV2_AZURE_18: MS managed Keys sufficient for testing
  #checkov:skip=CKV2_AZURE_1: MS Managed Keys beiong used
  #checkov:skip=CKV_AZURE_33: Queue logging policy not defined at this stage

  name                              = local.storageaccountname
  resource_group_name               = azurerm_resource_group.application.name
  location                          = azurerm_resource_group.application.location
  account_tier                      = "Standard"
  account_replication_type          = "LRS"
  infrastructure_encryption_enabled = true
  enable_https_traffic_only         = true
  min_tls_version                   = "TLS1_2"

  network_rules {
    default_action = "Deny"
  }

}

resource "azurerm_storage_table" "storage" {

  for_each = var.tables

  name                 = each.key
  storage_account_name = azurerm_storage_account.storage.name
}

resource "azurerm_key_vault_secret" "connection_string" {
  name         = "storageAccountConnectionString"
  value        = azurerm_storage_account.storage.primary_connection_string
  key_vault_id = azurerm_key_vault.az_key_vault.id
}


resource "azurerm_key_vault_secret" "access_key" {
  name         = "storageAccountAccessKey"
  value        = azurerm_storage_account.storage.primary_access_key
  key_vault_id = azurerm_key_vault.az_key_vault.id
}