resource "azurerm_resource_group" "application" {
  name     = "${locals.basename}-rg"
  location = var.location
}
