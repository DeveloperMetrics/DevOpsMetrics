#Login to Azure
#az login
cls

$apprefix ="DevopsMetrics2"
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
$storageName = "$($apprefixLC)$($environmentLC)$($regionShort)store".subString(0, 24)

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

Write-Host "here"

#Create resources
az group create --name $resourceGroup --location $region

az storage account create --name $storageName --resource-group $resourceGroup --sku $storageSKU --tags $tags
#az storage account show --name $storageName --resource-group $resourceGroup
#az storage account show --name devopsmetricsprodeustore --resource-group DevOpsMetrics
#az storage account delete --name $storageName --resource-group $resourceGroup

#Add tables to storage

#