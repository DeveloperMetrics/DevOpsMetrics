using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class MeanTimeToRestoreDA
    {
        public async Task<MeanTimeToRestoreModel> GetAzureMeanTimeToRestore(bool getSampleData,
                TableStorageAuth tableStorageAuth,
                string resourceGroup, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            if (getSampleData == false)
            {
                //Pull the events from the table storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                Newtonsoft.Json.Linq.JArray list = daTableStorage.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableMMTRRaw, resourceGroup);
                List<AzureAlert> alerts = JsonConvert.DeserializeObject<List<AzureAlert>>(list.ToString());

                //Compile the events
                List<MeanTimeToRestoreEvent> events = new List<MeanTimeToRestoreEvent>();
                int i = 0;
                foreach (AzureAlert item in alerts)
                {
                    i++;
                    MeanTimeToRestoreEvent newEvent = new MeanTimeToRestoreEvent
                    {
                        ResourceGroup = resourceGroup,
                        StartTime = item.timestamp,
                        ItemOrder = i
                    };
                    events.Add(newEvent);
                }
                //sort the list
                events = events.OrderBy(o => o.StartTime).ToList();

                //Pull together the results into a single model
                MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
                {
                    ResourceGroup = resourceGroup,
                    MeanTimeToRestoreEvents = events
                };
                return model;
            }
            else
            {
                //Return sample data
                MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
                {
                    IsAzureDevOps = true,
                    ResourceGroup = resourceGroup,
                    MeanTimeToRestoreEvents = GetSampleMMTREvents(resourceGroup),
                    MTTRAverageDurationInMinutes = 1f
                };
                return model;
            }
        }

        private List<MeanTimeToRestoreEvent> GetSampleMMTREvents(string resourceGroup)
        {
            List<MeanTimeToRestoreEvent> results = new List<MeanTimeToRestoreEvent>();
            MeanTimeToRestoreEvent item1 = new MeanTimeToRestoreEvent
            {
                ResourceGroup = resourceGroup,
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                ItemOrder = 1
            };
            results.Add(item1);
            MeanTimeToRestoreEvent item2 = new MeanTimeToRestoreEvent
            {
                ResourceGroup = resourceGroup,
                StartTime = DateTime.Now.AddDays(-5).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-5).AddMinutes(0),
                ItemOrder = 2
            };
            results.Add(item2);
            MeanTimeToRestoreEvent item3 = new MeanTimeToRestoreEvent
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-1),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                ItemOrder = 3
            };
            results.Add(item3);
            MeanTimeToRestoreEvent item4 = new MeanTimeToRestoreEvent
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                ItemOrder = 4
            };
            results.Add(item4);
            MeanTimeToRestoreEvent item5 = new MeanTimeToRestoreEvent
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-7),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                ItemOrder = 5
            };
            results.Add(item5);
            MeanTimeToRestoreEvent item6 = new MeanTimeToRestoreEvent
            {
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                ItemOrder = 6
            };
            results.Add(item6);

            return results;
        }

    }
}

