# az login
# az account set --subscription 65b8d298-e5bd-4735-912e-8b9c510c4e00


##
#secrets.AZURE_SP
#secrets.AZURE_SP_PROBOT
#secrets.AzureDevOpsPATToken
#secrets.AzureStorageConnectionString
#secrets.GITHUB_TOKEN
#secrets.GitHubClientId
#secrets.GitHubClientSecret
#secrets.KeyVaultClientId
#secrets.KeyVaultClientSecret

##
CLS
$resourceGroupName="DoraMetrics-wes-dev-rg"
$resourceLocation="westeurope"
$keyVaultName="dorametricswesdev"
$storageName="dorametricswesdev"
$hostingName="dorametrics-wes-devapp"
$appInsightsName="devops-prod-eu-appinsights"
$serviceName="dorametrics-wes-devfunc"
$websiteName="dorametrics-wes-devapp"
$functionName="dorametrics-wes-devfunc"
$administrationEmailAccount="pknw1@outlook.cm"
$fileRoot = "./src"
$templatesLocation="$fileRoot/DevOpsMetrics.Infrastructure/Templates"
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

#key vault
#Get user object id: https://docs.microsoft.com/en-us/cli/azure/ad/user?view=azure-cli-latest#az_ad_user_show
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
#if($error)
#{
#    #purge any existing key vault because of soft delete
#    Write-Host "Purging existing keyvault"
#    az keyvault purge --name $keyVaultName 
#    Write-Host "Creating keyvault, round 2"
#    az deployment group create --resource-group $resourceGroupName --name $keyVaultName --template-file "$templatesLocation\KeyVault.json" --parameters keyVaultName=$keyVaultName administratorUserPrincipalId=$administratorUserSid azureDevOpsPrincipalId=$azureDevOpsPrincipalId
#    $error.clear()
#}

#storage

#hosting
#app insights
#Write-Host "Setting value $ApplicationInsightsInstrumentationKey for $applicationInsightsInstrumentationKeyName to key vault"
#az keyvault secret set --vault-name $keyVaultName --name "$applicationInsightsInstrumentationKeyName" --value $ApplicationInsightsInstrumentationKey #Upload the secret into the key vault

#web service app service

#Deploy web service 
$dotnetPublishOutput = dotnet publish "$fileRoot/DevOpsMetrics.Service/DevOpsMetrics.Service.csproj" --configuration Debug --output "$fileRoot/DevOpsMetrics.Service/bin/webservice" 
Compress-Archive -Path "$fileRoot/DevOpsMetrics.Service/bin/webservice/*.*" -DestinationPath "$fileRoot/DevOpsMetrics.Service/bin/webservice.zip" -Force
#-------$serviceDeploymentOutput = az webapp deployment source config-zip --resource-group $resourceGroupName --name $serviceName --src "$fileRoot\DevOpsMetrics.Service\bin\webservice.zip"

#Set secrets into appsettings for web service
$configServiceSetOutput = az webapp config appsettings set --resource-group $resourceGroupName --name $serviceName --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$applicationInsightsInstrumentationKey" "AppSettings:KeyVaultURL=https://$keyVaultName.vault.azure.net/"
#$timing = -join($timing, "8. Web service created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
#Write-Host "8. Web service created: "$stopwatch.Elapsed.TotalSeconds

#function
#Set secrets into appsettings 


$timing = -join($timing, "11. All Done: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "11. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"
