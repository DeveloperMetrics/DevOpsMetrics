using System.Collections.Generic;

namespace DevOpsMetrics.Service.DataAccess
{
    public class Utility<T>
    {
        public List<T> GetLastNItems(List<T> items, int maxNumberOfItems)
        {
            if (items.Count <= maxNumberOfItems)
            {
                return items;
            }
            else
            {
                List<T> newItems = new List<T>();
                for (int i = items.Count - maxNumberOfItems; i < items.Count; i++)
                {
                    newItems.Add(items[i]);
                }
                return newItems;
            }
        }
    }
}
