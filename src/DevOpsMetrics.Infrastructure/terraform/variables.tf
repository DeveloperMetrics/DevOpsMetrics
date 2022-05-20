variable "environments" {
  type = map(any)

  default = {
    environment_name = "environment_name"
    location         = "ukwest"
  }
}

variable "location" {
  type    = string
  default = "ukwest"
}

variable "project_name" {
  type    = string
  default = "DoraMetrics"
}

