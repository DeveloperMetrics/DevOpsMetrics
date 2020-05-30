﻿using DevOpsMetrics.Core;
using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                string resourceGroup, bool isAzureDevOps, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            if (getSampleData == false)
            {
                //Pull the events from the table storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                Newtonsoft.Json.Linq.JArray list = daTableStorage.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableMTTRRaw, resourceGroup);
                List<AzureAlert> alerts = new List<AzureAlert>();
                //List<AzureAlert> alerts = JsonConvert.DeserializeObject<List<AzureAlert>>(list.ToString());
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
                List<MeanTimeToRestoreEvent> events = new List<MeanTimeToRestoreEvent>();

                //Loop through first finding the activated alerts
                int i = 0;
                List<AzureAlert> startingAlerts = alerts.Where(o => o.status == "Activated").ToList();
                foreach (AzureAlert item in startingAlerts)
                {
                    i++;
                    MeanTimeToRestoreEvent newEvent = new MeanTimeToRestoreEvent
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

                //Now loop through again, looking for the deactivated pairs
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
                            && endingAlerts[j].resourceGroupName == item.ResourceGroup)
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

                //Finally, process the percent calculation
                foreach (MeanTimeToRestoreEvent item in events)
                {
                    float interiumResult = ((item.MTTRDurationInHours / maxEventDuration) * 100f);
                    item.MTTRDurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                }

                //sort the final list (May not be needed due to the initial sort on the starting alerts)
                events = events.OrderBy(o => o.StartTime).ToList();

                MeanTimeToRestore mttr = new MeanTimeToRestore();
                float averageMTTR = CalculateMTTRDuration(events);
                //Pull together the results into a single model
                MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
                {
                    IsAzureDevOps = isAzureDevOps,
                    ResourceGroup = resourceGroup,
                    MeanTimeToRestoreEvents = events,
                    MTTRAverageDurationInHours = averageMTTR,
                    MTTRAverageDurationDescription = mttr.GetMeanTimeToRestoreRating(averageMTTR)
                };
                return model;
            }
            else
            {
                //Return sample data
                MeanTimeToRestore mttr = new MeanTimeToRestore();
                float averageMTTR = CalculateMTTRDuration(GetSampleMTTREvents(resourceGroup));
                MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
                {
                    IsAzureDevOps = isAzureDevOps,
                    ResourceGroup = resourceGroup,
                    MeanTimeToRestoreEvents = GetSampleMTTREvents(resourceGroup),
                    MTTRAverageDurationInHours = averageMTTR,
                    MTTRAverageDurationDescription = mttr.GetMeanTimeToRestoreRating(averageMTTR)
                };
                return model;
            }
        }

        private float CalculateMTTRDuration(List<MeanTimeToRestoreEvent> events)
        {
            float total = 0f;
            foreach (MeanTimeToRestoreEvent item in events)
            {
                total += item.MTTRDurationInHours;
            }
            float average = 0f;
            if (events.Count > 0)
            {
                average = total / events.Count;
            }
            return average;
        }

        private List<MeanTimeToRestoreEvent> GetSampleMTTREvents(string resourceGroup)
        {
            List<MeanTimeToRestoreEvent> results = new List<MeanTimeToRestoreEvent>();
            MeanTimeToRestoreEvent item1 = new MeanTimeToRestoreEvent
            {
                ResourceGroup = resourceGroup,
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                MTTRDurationPercent = 60,
                ItemOrder = 1
            };
            results.Add(item1);
            MeanTimeToRestoreEvent item2 = new MeanTimeToRestoreEvent
            {
                ResourceGroup = resourceGroup,
                StartTime = DateTime.Now.AddDays(-5).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-5).AddMinutes(0),
                MTTRDurationPercent = 80,
                ItemOrder = 2
            };
            results.Add(item2);
            MeanTimeToRestoreEvent item3 = new MeanTimeToRestoreEvent
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-1),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                MTTRDurationPercent = 20,
                ItemOrder = 3
            };
            results.Add(item3);
            MeanTimeToRestoreEvent item4 = new MeanTimeToRestoreEvent
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                MTTRDurationPercent = 60,
                ItemOrder = 4
            };
            results.Add(item4);
            MeanTimeToRestoreEvent item5 = new MeanTimeToRestoreEvent
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-7),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                MTTRDurationPercent = 100,
                ItemOrder = 5
            };
            results.Add(item5);
            MeanTimeToRestoreEvent item6 = new MeanTimeToRestoreEvent
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

