resource "azurerm_resource_group" "application" {
  name     = "${local.basename}-rg"
  location = var.location
}


resource "azurerm_role_assignment" "owner" {
  scope                = azurerm_resource_group.application.id
  role_definition_name = "Owner"
  principal_id         = "625b66d7-5b11-40fb-99ab-ba303c13ea88"
}