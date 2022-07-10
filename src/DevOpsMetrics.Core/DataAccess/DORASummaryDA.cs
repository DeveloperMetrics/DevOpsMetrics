using System.Collections.Generic;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.DataAccess
{
    public class DORASummaryDA
    {
        public static DORASummaryItem GetDORASummaryItem(TableStorageConfiguration tableStorageConfig,
                string owner, string repo)
        {
            DORASummaryItem model = null;
            AzureTableStorageDA da = new();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableDORASummaryItem, owner);
            List<DORASummaryItem> doraItems = JsonConvert.DeserializeObject<List<DORASummaryItem>>(list.ToString());
            foreach (DORASummaryItem item in doraItems)
            {
                if (item.Repo.ToLower() == repo.ToLower())
                {
                    model = item;
                    break;
                }
            }
            return model;
        }

    }
}
