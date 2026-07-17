using System.Collections.Generic;
using UnityEngine;

public static class InstantiateUtilities
{
    public static void PopulateItems<TData, TItem>(List<TData> dataList, List<TItem> itemList) where TItem : MonoBehaviour
    {
        if (itemList == null || itemList.Count == 0)
            return;

        TItem template = itemList[0];
        Transform parent = template.transform.parent;

        int toSpawn = dataList.Count - itemList.Count;
        if (toSpawn > 0)
        {
            for (int i = 0; i < toSpawn; i++)
            {
                TItem newItem = Object.Instantiate(template, parent);
                itemList.Add(newItem);
            }
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].gameObject.SetActive(i < dataList.Count);
        }
    }
    
    public static void PopulateItems<TItem>(int count, List<TItem> itemList) where TItem : MonoBehaviour
    {
        if (itemList == null || itemList.Count == 0)
            return;

        TItem template = itemList[0];
        Transform parent = template.transform.parent;

        int toSpawn = count - itemList.Count;
        if (toSpawn > 0)
        {
            for (int i = 0; i < toSpawn; i++)
            {
                TItem newItem = Object.Instantiate(template, parent);
                itemList.Add(newItem);
            }
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].gameObject.SetActive(i < count);
        }
    }
}
