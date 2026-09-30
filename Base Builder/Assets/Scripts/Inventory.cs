using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public Dictionary<string, int> Items { get; private set; }
    public int MaxWeight { get; private set; }

    public Inventory(int maxWeight)
    {
        Items = new();
        MaxWeight = maxWeight;
    }
    public Inventory(Inventory inv)
    {
        Items = new(inv.Items);
        MaxWeight = inv.MaxWeight;
    }

    public void Add(ResourceSO item, int amount)
    {
        string id = item.ID;
        if (!Items.ContainsKey(id))
            Items.Add(id, amount);

        Items[id] += amount;
        Items[id] = Mathf.Clamp(Items[id], 0, MaxWeight);
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
        Items[id] = Mathf.Clamp(Items[id], 0, MaxWeight);
    }
    public void ClearAll()
    {
        foreach(var id in Items.Keys)
            Items[id] = 0;
    }
}
