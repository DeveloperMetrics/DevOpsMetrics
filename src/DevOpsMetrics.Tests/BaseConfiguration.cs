using System;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests
{
    public class BaseConfiguration
    {
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            configBuilder.AddUserSecrets<BaseConfiguration>(true);
            Configuration = configBuilder.Build();

            string keyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string clientId = Configuration["AppSettings:KeyVaultClientId"];
            string clientSecret = Configuration["AppSettings:KeyVaultClientSecret"];
            string tenantId = Configuration["AppSettings:TenantId"];
            //AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            //KeyVaultClient keyVaultClient = new KeyVaultClient(
            //    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //config.AddAzureKeyVault(azureKeyVaultURL, keyVaultClient, new DefaultKeyVaultSecretManager());
            //configBuilder.AddAzureKeyVault(keyVaultURL, clientId, clientSecret);
            if (keyVaultURL != null && clientId != null && clientSecret != null && tenantId != null)
            {
                TokenCredential tokenCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                configBuilder.AddAzureKeyVault(new(keyVaultURL), tokenCredential);
                Configuration = configBuilder.Build();
            }
            else
            {
                throw new System.Exception("Missing configuration for Azure Key Vault");
            }
        }

    }
}
