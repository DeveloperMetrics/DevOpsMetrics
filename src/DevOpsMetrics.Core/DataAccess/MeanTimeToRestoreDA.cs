using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.Common;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Azure;
using DevOpsMetrics.Core.Models.Common;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.DataAccess
{
    public class MeanTimeToRestoreDA
    {
        public static async Task<MeanTimeToRestoreModel> GetAzureMeanTimeToRestore(bool getSampleData,
                TableStorageConfiguration tableStorageConfig,
                DevOpsPlatform targetDevOpsPlatform, string resourceGroup,
                int numberOfDays, int maxNumberOfItems)
        {
            ListUtility<MeanTimeToRestoreEvent> utility = new();
            if (getSampleData == false)
            {
                //If the user didn't specify a resource group, it comes in as null and causes havoc. Setting it as "" helps, it 
                if (resourceGroup == null)
                {
                    resourceGroup = "";
                }

                //Pull the events from the table storage
                AzureTableStorageDA daTableStorage = new();
                JArray list = await daTableStorage.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableMTTR, resourceGroup);
                List<AzureAlert> alerts = new();
                foreach (JToken item in list)
                {
                    alerts.Add(
                        new AzureAlert
                        {
                            name = item["data"]["context"]["name"].ToString(),
                            resourceGroupName = item["data"]["context"]["resourceGroupName"].ToString(),
                            resourceName = item["data"]["context"]["resourceName"].ToString(),
                            status = item["data"]["status"].ToString(),
                            timestamp = DateTime.Parse(item["data"]["context"]["timestamp"].ToString())
                        });
                }
                //sort the events by timestamp
                alerts = alerts.OrderBy(o => o.timestamp).ToList();

                //Compile the events,  looking for pairs, using the ordered data, and name, resource group name and resource name
                List<MeanTimeToRestoreEvent> events = new();

                //Loop through first finding the activated alerts
                int i = 0;
                List<AzureAlert> startingAlerts = alerts.Where(o => o.status == "Activated").ToList();
                foreach (AzureAlert item in startingAlerts)
                {
                    if (item.timestamp > DateTime.Now.AddDays(-numberOfDays))
                    {
                        i++;
                        MeanTimeToRestoreEvent newEvent = new()
                        {
                            Name = item.name,
                            Resource = item.resourceName,
                            ResourceGroup = item.resourceGroupName,
                            StartTime = item.timestamp,
                            Status = "inProgress",
                            ItemOrder = i
                        };
                        events.Add(newEvent);
                    }
                }

                //Now loop through again, looking for the deactivated matching pair
                float maxEventDuration = 0;
                List<AzureAlert> endingAlerts = alerts.Where(o => o.status == "Deactivated").ToList();
                foreach (MeanTimeToRestoreEvent item in events)
                {
                    //Search for the next matching deactivated alert
                    int foundItemIndex = -1;
                    for (int j = 0; j <= endingAlerts.Count - 1; j++)
                    {
                        if (endingAlerts[j].name == item.Name
                            && endingAlerts[j].resourceName == item.Resource
                            && endingAlerts[j].resourceGroupName == item.ResourceGroup
                            && endingAlerts[j].timestamp > item.StartTime)
                        {
                            item.EndTime = endingAlerts[j].timestamp;
                            item.Status = "completed";
                            foundItemIndex = j;
                            break;
                        }
                    }
                    if (foundItemIndex >= 0)
                    {
                        //Remove the found item from the list, so we don't use it again.
                        endingAlerts.RemoveAt(foundItemIndex);
                        if (item.MTTRDurationInHours > maxEventDuration)
                        {
                            maxEventDuration = item.MTTRDurationInHours;
                        }
                    }
                }

                //Calculate the MTTR metric
                MeanTimeToRestore mttr = new();
                List<KeyValuePair<DateTime, TimeSpan>> dateList = ConvertEventsToDateList(events);
                float averageMTTR = mttr.ProcessMeanTimeToRestore(dateList, numberOfDays);

                //Calculate the SLA metric
                SLA slaMetric = new();
                float sla = slaMetric.ProcessSLA(dateList, numberOfDays);

                //Filter the list for the UI, and sort the final list (May not be needed due to the initial sort on the starting alerts)
                List<MeanTimeToRestoreEvent> uiEvents = utility.GetLastNItems(events, maxNumberOfItems);
                uiEvents = uiEvents.OrderBy(o => o.StartTime).ToList();

                //Finally, process the percent calculation
                foreach (MeanTimeToRestoreEvent item in uiEvents)
                {
                    float interiumResult = ((item.MTTRDurationInHours / maxEventDuration) * 100f);
                    item.MTTRDurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                }

                //Pull together the results into a single model
                MeanTimeToRestoreModel model = new()
                {
                    TargetDevOpsPlatform = targetDevOpsPlatform,
                    ResourceGroup = resourceGroup,
                    MeanTimeToRestoreEvents = uiEvents,
                    MTTRAverageDurationInHours = averageMTTR,
                    MTTRAverageDurationDescription = MeanTimeToRestore.GetMeanTimeToRestoreRating(averageMTTR),
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = uiEvents.Count,
                    TotalItems = events.Count,
                    SLA = sla,
                    SLADescription = SLA.GetSLARating(sla)
                };
                return model;
            }
            else
            {
                //Get sample data
                MeanTimeToRestore mttr = new();
                List<MeanTimeToRestoreEvent> sampleEvents = GetSampleMTTREvents(resourceGroup);
                List<KeyValuePair<DateTime, TimeSpan>> dateList = ConvertEventsToDateList(sampleEvents);
                float averageMTTR = mttr.ProcessMeanTimeToRestore(dateList, numberOfDays);
                SLA slaMetric = new();
                float sla = slaMetric.ProcessSLA(dateList, numberOfDays);
                MeanTimeToRestoreModel model = new()
                {
                    TargetDevOpsPlatform = targetDevOpsPlatform,
                    ResourceGroup = resourceGroup,
                    MeanTimeToRestoreEvents = sampleEvents,
                    MTTRAverageDurationInHours = averageMTTR,
                    MTTRAverageDurationDescription = MeanTimeToRestore.GetMeanTimeToRestoreRating(averageMTTR),
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = sampleEvents.Count,
                    TotalItems = sampleEvents.Count,
                    SLA = sla,
                    SLADescription = SLA.GetSLARating(sla)
                };
                return model;
            }
        }

        private static List<KeyValuePair<DateTime, TimeSpan>> ConvertEventsToDateList(List<MeanTimeToRestoreEvent> events)
        {
            List<KeyValuePair<DateTime, TimeSpan>> dateList = new();
            foreach (MeanTimeToRestoreEvent item in events)
            {
                if (item.Status == "completed" || item.Status == "Completed")
                {
                    dateList.Add(new KeyValuePair<DateTime, TimeSpan>(item.StartTime, item.EndTime - item.StartTime));
                }
                else
                {
                    Console.Write("Unknown status: " + item.Status);
                }
            }

            return dateList;
        }

        //private float CalculateMTTRDuration(List<MeanTimeToRestoreEvent> events)
        //{
        //    float total = 0f;
        //    foreach (MeanTimeToRestoreEvent item in events)
        //    {
        //        total += item.MTTRDurationInHours;
        //    }
        //    float average = 0f;
        //    if (events.Count > 0)
        //    {
        //        average = total / events.Count;
        //    }
        //    return average;
        //}

        //Return a sample dataset to help with testing
        private static List<MeanTimeToRestoreEvent> GetSampleMTTREvents(string resourceGroup)
        {
            List<MeanTimeToRestoreEvent> results = new();
            MeanTimeToRestoreEvent item1 = new()
            {
                ResourceGroup = resourceGroup,
                Name = "Name1",
                Resource = "Resource1",
                Status = "Completed",
                Url = "https://mttr.com",
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                MTTRDurationPercent = 60,
                ItemOrder = 1
            };
            results.Add(item1);
            MeanTimeToRestoreEvent item2 = new()
            {
                ResourceGroup = resourceGroup,
                StartTime = DateTime.Now.AddDays(-5).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-5).AddMinutes(0),
                MTTRDurationPercent = 80,
                ItemOrder = 2
            };
            results.Add(item2);
            MeanTimeToRestoreEvent item3 = new()
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-1),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                MTTRDurationPercent = 20,
                ItemOrder = 3
            };
            results.Add(item3);
            MeanTimeToRestoreEvent item4 = new()
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                MTTRDurationPercent = 60,
                ItemOrder = 4
            };
            results.Add(item4);
            MeanTimeToRestoreEvent item5 = new()
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-7),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                MTTRDurationPercent = 100,
                ItemOrder = 5
            };
            results.Add(item5);
            MeanTimeToRestoreEvent item6 = new()
            {
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                MTTRDurationPercent = 80,
                ItemOrder = 6
            };
            results.Add(item6);

            return results;
        }

    }
}

