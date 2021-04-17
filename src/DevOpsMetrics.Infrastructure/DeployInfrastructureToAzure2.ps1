# DeployInfrastructureToAzure2.ps1 -resourceGroupName devopswestus -resourceLocation westus -storageName devopsprodwustorage -keyVaultName "devopsmetrics-prod-wu-keyvault" hostingName "devopsmetrics-prod-wu-hosting" 
# -appInsightsName "devopsmetrics-prod-wu-appinsights" -websiteName "devopsmetrics-prod-wu-web" -serviceName "devopsmetrics-prod-wu-service" -functionName "devopsmetrics-prod-wu-function" -templatesLocation "C:\Users\samsmit\source\repos\DevOpsMetrics\src\DevOpsMetrics.Infrastructure\Templates"

param
(   
    [string] $resourceGroupName,
	[string] $resourceLocation,
	[string] $storageName,
    [string] $keyVaultName,
	[string] $hostingName,
	[string] $appInsightsName,
	[string] $websiteName,
	[string] $serviceName,
	[string] $functionName,
	[string] $templatesLocation
)

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$timing = ""
$timing = -join($timing, "1. Deployment started: ", $stopwatch.Elapsed.TotalSeconds, "`n")
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Parameters:"
Write-Host "resourceGroupName: $resourceGroupName"
Write-Host "resourceLocation: $resourceLocation"
Write-Host "storageName: $storageName"
Write-Host "hostingName: $hostingName"
Write-Host "appInsightsName: $appInsightsName"
Write-Host "websiteName: $websiteName"
Write-Host "serviceName: $serviceName"
Write-Host "functionName: $functionName"

#Variables
if ($storageName.Length -gt 24)
{
    Write-Host "Storage account name must be 3-24 characters in length"
    Break
}

$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds
