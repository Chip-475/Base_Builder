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
        return (item, amount);
    }
    public int GetQuantityOf(ResourceSO item)
    {
        return Items[item];
    }

    public void ClearAll()
    {
        foreach(var id in Items.Keys)
            Items[id] = 0;
    }
}
