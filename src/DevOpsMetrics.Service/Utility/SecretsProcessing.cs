using System.Text.RegularExpressions;

namespace DevOpsMetrics.Service.Utility
{
    public static class SecretsProcessing
    {
        //https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/resource-name-rules#microsoftkeyvault
        //Only Alphanumerics and hyphens allowed
        public static string CleanKey(string name)
        {
            return Regex.Replace(name, @"[^a-zA-Z0-9]+", "-").Trim('-');
        }

    }
}
