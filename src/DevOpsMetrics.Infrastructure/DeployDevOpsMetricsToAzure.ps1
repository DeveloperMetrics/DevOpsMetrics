#Login to Azure
#az login
cls

$apprefix ="Devops2"
$region = "East US" #East US
$regionShort = "eu" #East US
$environment = "Prod"
$storageSKU = "Standard_LRS"
$tags = "" #Space-separated tags: key[=value] [key[=value] ...]. Use "" to clear existing tags.

$resourceGroup = "$apprefix$Environment"
$apprefixLC = $apprefix.ToLower()
$environmentLC = $Environment.ToLower()

$functionName = "$apprefixLC-$environmentLC-$regionShort-function"
$serviceName = "$apprefixLC-$environmentLC-$regionShort-service"
$webName = "$apprefixLC-$environmentLC-$regionShort-web"
$storageName = "$($apprefixLC)$($environmentLC)$($regionShort)store"
if ($storageName.Length -gt 24)
{
    $storageName = $storageName.subString(0, 24)
}

$functionName
$serviceName
$webName
$storageName

#Validate names
$storageValidationJson = az storage account check-name --name $storageName
$storageValidation = $storageValidationJson | ConvertFrom-Json    
if ($storageValidation.nameAvailable -eq $false) 
{
    Write-Host "Storage account $storageName not available"
    break; #stop the script from running
}                          

#Create resource group
az group create --name $resourceGroup --location $region

#storage account
az storage account create --name $storageName --resource-group $resourceGroup --sku $storageSKU --tags $tags
#az storage account show --name $storageName --resource-group $resourceGroup
#az storage account delete --name $storageName --resource-group $resourceGroup

$connectionStringJson = az storage account show-connection-string --name $storageName --resource-group $resourceGroup
$connectionString = $connectionStringJson | ConvertFrom-Json

#Add tables to storage
$AzureStorageAccountContainerAzureDevOpsBuilds = "DevOpsAzureDevOpsBuilds"
$AzureStorageAccountContainerAzureDevOpsPRs = "DevOpsAzureDevOpsPRs"
$AzureStorageAccountContainerAzureDevOpsPRCommits = "DevOpsAzureDevOpsPRCommits"
$AzureStorageAccountContainerAzureDevOpsSettings = "DevOpsAzureDevOpsSettings"
$AzureStorageAccountContainerGitHubRuns = "DevOpsGitHubRuns"
$AzureStorageAccountContainerGitHubPRs = "DevOpsGitHubPRs"
$AzureStorageAccountContainerGitHubPRCommits = "DevOpsGitHubPRCommits"
$AzureStorageAccountContainerGitHubSettings = "DevOpsGitHubSettings"
$AzureStorageAccountContainerMTTR = "DevOpsMTTR"
$AzureStorageAccountContainerChangeFailureRate = "DevOpsChangeFailureRate"
$AzureStorageAccountContainerTableLog = "DevOpsLog"

az storage table create --name $AzureStorageAccountContainerAzureDevOpsBuilds --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerAzureDevOpsPRs --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerAzureDevOpsPRCommits --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerAzureDevOpsSettings --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerGitHubRuns --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerGitHubPRs --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerGitHubPRCommits --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerGitHubSettings --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerMTTR --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerChangeFailureRate --connection-string $connectionString 
az storage table create --name $AzureStorageAccountContainerTableLog --connection-string $connectionString 


#Create a web plan
#Create app insights resource

#Create a web app for the web api
#Set configurations in the web app

#Create a web app for the website
#Set configurations in the web app

#Create a function
#Set configurations in the web app

