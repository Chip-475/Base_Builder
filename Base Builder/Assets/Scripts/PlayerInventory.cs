using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public Dictionary<string, int> Items { get; private set; }

    public void Add(ResourceSO item, int amount)
    {
        string id = item.ID;
        if (!Items.ContainsKey(id))
            Items.Add(id, 0);

        Items[id] += amount;
    }
    public void Remove(ResourceSO item, int amount)
    {
        string id = item.ID;
        if (!Items.ContainsKey(id))
        {
            Debug.Log($"No item with ID: {id} found in inventory.");
            return;
        }

        Items[id] -= amount;
    }
    public int GetQuantityOf(string itemID)
    {
        return Items[itemID];
    }

    public void ClearAll()
    {
        foreach(var id in Items.Keys)
            Items[id] = 0;
    }
}
