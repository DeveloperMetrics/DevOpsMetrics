using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
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

                    // Load Secrects into Configuration!
                    builder.AddAzureKeyVault(new Uri(configuration["AppSettings:KeyVaultURL"]), new DefaultAzureCredential());

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
