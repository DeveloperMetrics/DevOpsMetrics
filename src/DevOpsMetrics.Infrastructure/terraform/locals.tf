locals {
  environment        = terraform.workspace
  basename           = "${var.project_name}-${substr(var.location, 0, 3)}-${var.environment}"
  keyvaultname       = lower(replace(local.basename, "-", ""))
  storageaccountname = lower(replace(local.basename, "-", ""))

}

