using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.DataAccess
{
    public class DORASummaryDA
    {
        public static async Task<List<DORASummaryItem>> GetDORASummaryItems(TableStorageConfiguration tableStorageConfig,
                string owner)
        {
            AzureTableStorageDA da = new();
            JArray list = await da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableDORASummaryItem, owner);
            List<DORASummaryItem> doraItems = JsonConvert.DeserializeObject<List<DORASummaryItem>>(list.ToString());
            return doraItems;
        }

        public static async Task<DORASummaryItem> GetDORASummaryItem(TableStorageConfiguration tableStorageConfig,
                string owner, string project, string repo)
        {
            DORASummaryItem result = null;
            List<DORASummaryItem> doraItems = await GetDORASummaryItems(tableStorageConfig, owner);
            if (doraItems != null)
            {
                foreach (DORASummaryItem item in doraItems)
                {
                    if ((project != null && item.Project == project) || (item.Repo?.ToLower() == repo.ToLower()))
                    {
                        result = item;
                        break;
                    }
                }
            }
            return result;
        }

    }
}
