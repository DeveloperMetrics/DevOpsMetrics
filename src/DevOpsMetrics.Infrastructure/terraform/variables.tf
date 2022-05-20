variable "location" {
  type    = string
  default = "ukwest"
}

variable "project_name" {
  type    = string
  default = "DoraMetrics"
}

variable "environment" {
  type = string
}

variable "tables" {
  type    = map(string)
  default = ["AzureDevOpsBuilds", "AzureDevOpsPRCommits", "AzureDevOpsPRs", "AzureDevOpsSettings", "ChangeFailureRate", "GitHubPRCommits", "GitHubPRs", "GitHubRuns", "GitHubSettings", "Log", "MTTR"]
}