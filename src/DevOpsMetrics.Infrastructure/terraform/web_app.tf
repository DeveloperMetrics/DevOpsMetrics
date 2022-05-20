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

  site_config {
    dotnet_framework_version = "v5.0"
    default_documents = ["Default.htm",
      "Default.html",
      "Default.asp",
      "index.htm",
      "index.html",
      "iisstart.htm",
      "default.aspx",
      "index.php",
    "hostingstart.html"]
  }

}


resource "azurerm_app_service" "web" {
  name                = "${local.basename}-web"
  location            = azurerm_resource_group.application.location
  resource_group_name = azurerm_resource_group.application.name
  app_service_plan_id = azurerm_app_service_plan.app.id
}