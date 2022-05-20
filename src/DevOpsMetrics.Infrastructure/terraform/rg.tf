resource "azurerm_resource_group" "application" {
  name     = "${local.basename}-rg"
  location = var.location
}
