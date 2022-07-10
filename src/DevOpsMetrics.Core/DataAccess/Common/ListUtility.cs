using System.Collections.Generic;

namespace DevOpsMetrics.Core.DataAccess.Common
{
    public class ListUtility<T>
    {
        //Get the last items from a list
        public List<T> GetLastNItems(List<T> items, int numberOfItems)
        {
            if (items.Count <= numberOfItems)
            {
                return items;
            }
            else
            {
                List<T> newItems = new();
                for (int i = items.Count - numberOfItems; i < items.Count; i++)
                {
                    newItems.Add(items[i]);
                }
                return newItems;
            }
        }

    }
}
