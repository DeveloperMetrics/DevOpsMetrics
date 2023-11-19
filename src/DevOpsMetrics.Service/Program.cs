using Microsoft.AspNetCore.Hosting;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DevOpsMetrics.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    //Load the appsettings.json configuration file
                    IConfigurationRoot configuration = builder.Build();

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>();
                        configuration = builder.Build();
                    }

                    //Load a connection to our Azure key vault instance
                    string azureKeyVaultURL = configuration["AppSettings:KeyVaultURL"];
                    string clientId = configuration["AppSettings:KeyVaultClientId"];
                    string clientSecret = configuration["AppSettings:KeyVaultClientSecret"];
                    string tenantId = configuration["AppSettings:TenantId"];
                    //AzureServiceTokenProvider azureServiceTokenProvider = new();
                    //KeyVaultClient keyVaultClient = new(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    //builder.AddAzureKeyVault(keyVaultURL, keyVaultClient, new DefaultKeyVaultSecretManager());
                    //builder.AddAzureKeyVault(keyVaultURL, clientId, clientSecret);
                    //configuration = builder.Build();

                    if (azureKeyVaultURL != null && clientId != null && clientSecret != null && tenantId != null)
                    {
                        TokenCredential tokenCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                        builder.AddAzureKeyVault(new(azureKeyVaultURL), tokenCredential);
                    }
                    else
                    {
                        throw new System.Exception("Missing configuration for Azure Key Vault");
                    }


                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
