using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Newtonsoft.Json;

namespace DevOpsMetrics.Core.DataAccess
{
    public class DORASummaryDA
    {
        public async Task<DORASummaryItem> GetDORASummaryItem(TableStorageConfiguration tableStorageConfig,
                string owner, string repo)
        {
            DORASummaryItem model = null;
            AzureTableStorageDA da = new();
            Newtonsoft.Json.Linq.JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableDORASummaryItem, repo);

            List<DORASummaryItem> doraItems = JsonConvert.DeserializeObject<List<DORASummaryItem>>(list.ToString());
            foreach (DORASummaryItem item in doraItems)
            {
                if (item.Repo == repo)
                {
                    model = item;
                    break;
                }
            }

            return model;
        }

    }
}
