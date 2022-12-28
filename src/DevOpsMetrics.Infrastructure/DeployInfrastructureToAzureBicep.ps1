# Run the following commands before running the deploy script, and adjust your subscription ID
## az login
## az account set --subscription afb8f550-216d-4848-b6f1-73b1bbf58f1e
CLS
# Adjust these settings!
## This is a suffix to uniquely identify your resources
$suffix="rm3"
## Retrieve this from Azure AD
$administrationEmailAccount="rodrigo.moreirao_microsoft.com#EXT#@fdpo.onmicrosoft.com"
$fileRoot = "C:\LocalDev\github\DevOpsMetrics\src"

$resourceGroupName="devopsmetrics$suffix"
$resourceLocation="eastus"
$keyVaultName="devops-prod-eu-vault$suffix"
$storageName="devopsprodeustorage$suffix"
$hostingName="devops-prod-eu-hosting$suffix"
$appInsightsName="devops-prod-eu-appinsights$suffix"
$serviceName="devops-prod-eu-service$suffix"
$websiteName="devops-prod-eu-web$suffix"
$functionName="devops-prod-eu-function$suffix"
$templatesLocation="$fileRoot\DevOpsMetrics.Infrastructure\Templates"
$error.clear()

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$timing = ""
$timing = -join($timing, "1. Deployment started: ", $stopwatch.Elapsed.TotalSeconds, "`n")
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Parameters:"
Write-Host "resourceGroupName: $resourceGroupName"
Write-Host "resourceLocation: $resourceLocation"
Write-Host "keyVaultName: $keyVaultName"
Write-Host "storageName: $storageName"
Write-Host "hostingName: $hostingName"
Write-Host "appInsightsName: $appInsightsName"
Write-Host "serviceName: $serviceName"
Write-Host "websiteName: $websiteName"
Write-Host "functionName: $functionName"
Write-Host "administrationEmailAccount: $administrationEmailAccount"

#Variables
if ($storageName.Length -gt 24)
{
    Write-Host "Storage account name must be 3-24 characters in length"
    Break
}
if ($keyVaultName.Length -gt 24)
{
    Write-Host "Key vault name must be 3-24 characters in length"
    Break
}

$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#Resource group
$resourcegroupDeployment = az group create --location $resourceLocation --name $resourceGroupName
$timing = -join($timing, "3. Resource group created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "3. Resource group created: "$stopwatch.Elapsed.TotalSeconds

#key vault
#Get user object id: https://docs.microsoft.com/en-us/cli/azure/ad/user?view=azure-cli-latest#az_ad_user_show
$userJson = az ad user show --id $administrationEmailAccount
$user = $userJson | ConvertFrom-Json
$administratorUserPrincipalId = $user.id
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
az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.bicep" --parameters keyVaultName=$keyVaultName administratorUserPrincipalId=$administratorUserPrincipalId

$timing = -join($timing, "4. Key vault created:: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "4. Key vault created: "$stopwatch.Elapsed.TotalSeconds

#storage
$storageOutput = az deployment group create --resource-group $resourceGroupName --name $storageName --template-file "$templatesLocation\Storage.bicep" --parameters storageAccountName=$storageName resourceGroupName=$resourceGroupName
$storageJSON = $storageOutput | ConvertFrom-Json
$storageAccountConnectionString = $storageJSON.properties.outputs.storageAccountConnectionString.value
Write-Host "Setting value storageAccountConnectionString to key vault"
az keyvault secret set --vault-name $keyVaultName --name "AppSettings--AzureStorageAccountConfigurationString" --value $storageAccountConnectionString 
Write-Host "storageAccountAccessKey: "$storageAccountConnectionString
$timing = -join($timing, "5. Storage created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "5. Storage created: "$stopwatch.Elapsed.TotalSeconds

#hosting
az deployment group create --resource-group $resourceGroupName --name $hostingName --template-file "$templatesLocation\WebHosting.bicep" --parameters hostingPlanName=$hostingName actionGroupName=$actionGroupName 
$timing = -join($timing, "6. Web hosting created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "6. Web hosting created: "$stopwatch.Elapsed.TotalSeconds

#app insights
$applicationInsightsOutput = az deployment group create --resource-group $resourceGroupName --name $appInsightsName --template-file "$templatesLocation\ApplicationInsights.bicep" --parameters applicationInsightsName=$appInsightsName 
$applicationInsightsJSON = $applicationInsightsOutput | ConvertFrom-Json
$applicationInsightsInstrumentationKey = $applicationInsightsJSON.properties.outputs.applicationInsightsInstrumentationKeyOutput.value
#Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
#az keyvault secret set --vault-name $keyVaultName --name "$applicationInsightsInstrumentationKeyName" --value $ApplicationInsightsInstrumentationKey #Upload the secret into the key vault
Write-Host "applicationInsightsInstrumentationKey: "$applicationInsightsInstrumentationKey
$timing = -join($timing, "7. Application created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "7. Application insights created: "$stopwatch.Elapsed.TotalSeconds

#web service app service
$webserviceOutput = az deployment group create --resource-group $resourceGroupName --name $serviceName --template-file "$templatesLocation\Website.bicep" --parameters webSiteName=$serviceName hostingPlanName=$hostingName

#Deploy web service 
$dotnetPublishOutput = dotnet publish "$fileRoot\DevOpsMetrics.Service\DevOpsMetrics.Service.csproj" --configuration Debug --output "$fileRoot\DevOpsMetrics.Service\bin\webservice" 
Compress-Archive -Path "$fileRoot\DevOpsMetrics.Service\bin\webservice\*.*" -DestinationPath "$fileRoot\DevOpsMetrics.Service\bin\webservice.zip" -Force
$serviceDeploymentOutput = az webapp deployment source config-zip --resource-group $resourceGroupName --name $serviceName --src "$fileRoot\DevOpsMetrics.Service\bin\webservice.zip"

#Set secrets into appsettings for web service
Write-Host "Setting appsettings $appInsightsName connectionString: $applicationInsightsInstrumentationKey"
$configServiceSetOutput = az webapp config appsettings set --resource-group $resourceGroupName --name $serviceName --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsInstrumentationKey" "AppSettings:KeyVaultURL=https://$keyVaultName.vault.azure.net/"
$timing = -join($timing, "8. Web service created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "8. Web service created: "$stopwatch.Elapsed.TotalSeconds

#Web site
$websiteOutput = az deployment group create --resource-group $resourceGroupName --name $webSiteName --template-file "$templatesLocation\Website.bicep" --parameters webSiteName=$webSiteName hostingPlanName=$hostingName
#Set secrets into appsettings 
Write-Host "Setting appsettings $appInsightsName connectionString: $applicationInsightsInstrumentationKey"
$configWebSetOutput = az webapp config appsettings set --resource-group $resourceGroupName --name $webSiteName --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsInstrumentationKey" #--slot production
$timing = -join($timing, "9. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "9. Website created: "$stopwatch.Elapsed.TotalSeconds

#function
$functionOutput = az deployment group create --resource-group $resourceGroupName --name $functionName --template-file "$templatesLocation\function.bicep" --parameters webSiteName=$functionName hostingPlanName=$hostingName 
#Set secrets into appsettings 
Write-Host "Setting appsettings $appInsightsName connectionString: $applicationInsightsInstrumentationKey"
$configFunctionOutput = az webapp config appsettings set --resource-group $resourceGroupName --name $webSiteName --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsInstrumentationKey" #--slot production
$timing = -join($timing, "10. Website created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "10. Website created: "$stopwatch.Elapsed.TotalSeconds


$timing = -join($timing, "11. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "11. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"
