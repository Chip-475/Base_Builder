using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory
{
    Dictionary<ResourceSO, int> items = new();
    public void AddItem(ResourceSO item, int quantity)
    {

        if (!items.ContainsKey(item))
            items.Add(item, 0);
        items[item] += quantity;


    }
    public void RemoveItem(ResourceSO item, int quantity)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= quantity;
        }
    }
}
