using UnityEngine;
using System.Collections.Generic;

public class DepotInventory
{
    private Dictionary<ResourceSO, int> deposit = new();

    public (ResourceSO resource, int quantity) AddItem(ResourceSO resource, int quantity)
    {
        if (deposit.ContainsKey(resource))
            deposit[resource] += quantity;
        else
            deposit[resource] = quantity;

        return (resource, deposit[resource]);
    }

    public (ResourceSO resource, int quantity) RemoveItem(ResourceSO resource, int quantity)
    {
        if (!deposit.TryGetValue(resource, out int currentQuantity))
            return (resource, 0);

        int removedQuantity = Mathf.Min(quantity, currentQuantity);
        int newQuantity = currentQuantity - removedQuantity;

        if (newQuantity <= 0)
            deposit.Remove(resource);
        else
            deposit[resource] = newQuantity;

        return (resource, removedQuantity);
    }

    public int GetQuantity(ResourceSO resource)
    {
        if (deposit.TryGetValue(resource, out int quantity))
            return quantity;

        return 0;
    }
}