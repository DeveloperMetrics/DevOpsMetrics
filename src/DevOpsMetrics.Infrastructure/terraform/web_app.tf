resource "azurerm_app_service_plan" "app" {
  name                = "${local.basename}_web_service_plan"
  location            = azurerm_resource_group.application.location
  resource_group_name = azurerm_resource_group.application.name

  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_app_service" "service" {
  name                = "${local.basename}-service"
  location            = azurerm_resource_group.application.location
  resource_group_name = azurerm_resource_group.application.name
  app_service_plan_id = azurerm_app_service_plan.app.id
}


resource "azurerm_app_service" "web" {
  name                = "${local.basename}-web"
  location            = azurerm_resource_group.application.location
  resource_group_name = azurerm_resource_group.application.name
  app_service_plan_id = azurerm_app_service_plan.app.id
}