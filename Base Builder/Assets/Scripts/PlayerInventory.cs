using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public Dictionary<ResourceSO, int> Items { get; private set; }

    public void Add(ResourceSO item, int amount)
    {
        if (!Items.ContainsKey(item))
            Items.Add(item, 0);

        Items[item] += amount;
    }
    public (ResourceSO item, int amount) Remove(ResourceSO item, int amount)
    {
        if (!Items.ContainsKey(item))
        {
            Debug.Log($"No item with ID: {item} found in inventory.");
            return (null, 0);
        }

        Items[item] -= amount;
        Items[item] = Mathf.Clamp(Items[item], 0, 999);
        return (item, amount);
    }
    public int GetQuantityOf(ResourceSO item)
    {
        if (!Items.ContainsKey(item))
            return 0;
        return Items[item];
    }

    public void ClearAll()
    {
        foreach(var id in Items.Keys)
            Items[id] = 0;
    }
}
