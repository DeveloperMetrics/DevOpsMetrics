# C:\Users\samsmit\source\repos\DevOpsMetrics\src\DevOpsMetrics.Infrastructure\DeployInfrastructureToAzure.ps1 -resourceGroupName devopswestus -resourceLocation westus -storageName devopsprodwustorage -keyVaultName "devopsmetrics-prod-wu-keyvault" hostingName "devopsmetrics-prod-wu-hosting" -appInsightsName "devopsmetrics-prod-wu-appinsights" -websiteName "devopsmetrics-prod-wu-web" -serviceName "devopsmetrics-prod-wu-service" -functionName "devopsmetrics-prod-wu-function" -templatesLocation "C:\Users\samsmit\source\repos\DevOpsMetrics\src\DevOpsMetrics.Infrastructure\Templates"

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

#Resource group
az group create --location $resourceLocation --name $resourceGroupName
$timing = -join($timing, "3. Resource group created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Resource group created: "$stopwatch.Elapsed.TotalSeconds

#key vault
#Get all deleted key vault names. If it matches, purge it
#$results = az keyvault list-deleted --subscription 07db7d0b-a6cb-4e58-b07e-e1d541c39f5b
#$results = $results | ConvertFrom-Json
#if ($results -ne $null -and $results.Length -gt 0)
#{
#    #if we have purged keyvaults, search to see if we've used this name before and purge it
#    foreach($purgedKV in $results) {
#        if ($purgedKV.name -eq $keyVaultName)
#        {
#            Write-Host "Purging existing keyvault" $purgedKV.name         
#            az keyvault purge --name $keyVaultName                     
#        }
#    }
#}
az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.json" --parameters keyVaultName=$keyVaultName
#if($error)
#{
#    #purge any existing key vault because of soft delete
#    Write-Host "Purging existing keyvault"
#    az keyvault purge --name $keyVaultName 
#    Write-Host "Creating keyvault, round 2"
#    az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.json" --parameters keyVaultName=$keyVaultName administratorUserPrincipalId=$administratorUserSid azureDevOpsPrincipalId=$azureDevOpsPrincipalId
#    $error.clear()
#}
$timing = -join($timing, "4. Key vault created:: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. Key vault created: "$stopwatch.Elapsed.TotalSeconds

#storage
$storageOutput = az deployment group create --resource-group $resourceGroupName --name $storageAccountName --template-file "$templatesLocation\Storage.json" --parameters storageAccountName=$storageAccountName resourceGroupName=$resourceGroupName
$storageJSON = $storageOutput | ConvertFrom-Json
$storageAccountConnectionString = $storageJSON.properties.outputs.storageAccountConnectionString.value
Write-Host "Setting value storageAccountConnectionString to key vault"
az keyvault secret set --vault-name $keyVaultName --name "storageAccountConnectionString" --value storageAccountConnectionString 
Write-Host "storageAccountAccessKey: "$storageAccountAccessKey
$timing = -join($timing, "5. Storage created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "5. Storage created: "$stopwatch.Elapsed.TotalSeconds

#hosting
az deployment group create --resource-group $resourceGroupName --name $webhostingName --template-file "$templatesLocation\WebHosting.json" --parameters hostingPlanName=$webhostingName actionGroupName=$actionGroupName 
$timing = -join($timing, "6. Web hosting created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. Web hosting created: "$stopwatch.Elapsed.TotalSeconds

#app insights
$applicationInsightsOutput = az deployment group create --resource-group $resourceGroupName --name $applicationInsightsName --template-file "$templatesLocation\ApplicationInsights.json" --parameters applicationInsightsName=$applicationInsightsName applicationInsightsAvailablityTestName="$applicationInsightsAvailablityTestName" websiteDomainName=$websiteDomainName 
$applicationInsightsJSON = $applicationInsightsOutput | ConvertFrom-Json
$applicationInsightsInstrumentationKey = $applicationInsightsJSON.properties.outputs.applicationInsightsInstrumentationKeyOutput.value
#Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
#az keyvault secret set --vault-name $keyVaultName --name "$applicationInsightsInstrumentationKeyName" --value $ApplicationInsightsInstrumentationKey #Upload the secret into the key vault
Write-Host "applicationInsightsInstrumentationKey: "$applicationInsightsInstrumentationKey
$timing = -join($timing, "7. Application created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "7. Application insights created: "$stopwatch.Elapsed.TotalSeconds

#web service
az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName
#Set secrets into appsettings 
Write-Host "Setting appsettings $applicationInsightsName connectionString: $applicationInsightsInstrumentationKey"
az webapp config appsettings set --resource-group $resourceGroupName --name $webSiteName --slot staging --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsInstrumentationKey" 
$timing = -join($timing, "8. Web service created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "8. Web service created: "$stopwatch.Elapsed.TotalSeconds

#Web site
az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.json" --parameters webSiteName=$webSiteName hostingPlanName=$webhostingName
#Set secrets into appsettings 
Write-Host "Setting appsettings $applicationInsightsName connectionString: $applicationInsightsInstrumentationKey"
az webapp config appsettings set --resource-group $resourceGroupName --name $webSiteName --slot staging --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsInstrumentationKey" 
$timing = -join($timing, "9. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "9. Website created: "$stopwatch.Elapsed.TotalSeconds

#function
az deployment group create --resource-group $resourceGroupName --name $functionName --template-file "$templatesLocation\function.json" --parameters webSiteName=$functionName hostingPlanName=$webhostingName 
#Set secrets into appsettings 
Write-Host "Setting appsettings $applicationInsightsName connectionString: $applicationInsightsInstrumentationKey"
az webapp config appsettings set --resource-group $resourceGroupName --name $webSiteName --slot staging --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsInstrumentationKey" 
$timing = -join($timing, "10. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "10. Website created: "$stopwatch.Elapsed.TotalSeconds

$timing = -join($timing, "11. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "11. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"