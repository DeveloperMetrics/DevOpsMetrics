namespace DevOpsMetrics.Core.Models.AzureDevOps
{
    public class AzureDevOpsSettings
    {
        public string RowKey
        {
            get; set;
        }
        public string Organization
        {
            get; set;
        }
        public string Project
        {
            get; set;
        }
        public string Repository
        {
            get; set;
        }
        public string Branch
        {
            get; set;
        }
        public string BuildName
        {
            get; set;
        }
        public string BuildId
        {
            get; set;
        }
        public string ProductionResourceGroup
        {
            get; set;
        }
        public int ItemOrder
        {
            get; set;
        }
        public bool ShowSetting
        {
            get; set;
        }
    }
}
