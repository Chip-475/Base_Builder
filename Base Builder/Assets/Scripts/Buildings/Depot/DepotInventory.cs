using System;
using UnityEngine;
using System.Collections.Generic;

public class DepotInventory
{
    private readonly Dictionary<ResourceSO, int> deposit = new();

    public event Action<ResourceSO, int> QuantityChanged;

    public (ResourceSO resource, int quantity) AddItem(ResourceSO resource, int quantity)
    {
        if (!IsValidRequest(resource, quantity))
            return (resource, GetQuantity(resource));

        deposit.TryGetValue(resource, out int currentQuantity);
        if (currentQuantity > int.MaxValue - quantity)
        {
            return (resource, currentQuantity);
        }

        int newQuantity = currentQuantity + quantity;
        deposit[resource] = newQuantity;
        if (QuantityChanged != null)
            QuantityChanged(resource, newQuantity);

        return (resource, newQuantity);
    }

    public (ResourceSO resource, int quantity) RemoveItem(ResourceSO resource, int quantity)
    {
        if (!IsValidRequest(resource, quantity))
            return (resource, 0);

        if (!deposit.TryGetValue(resource, out int currentQuantity))
            return (resource, 0);

        int removedQuantity = Mathf.Min(quantity, currentQuantity);
        int newQuantity = currentQuantity - removedQuantity;

        if (newQuantity <= 0)
            deposit.Remove(resource);
        else
            deposit[resource] = newQuantity;

        if (QuantityChanged != null)
            QuantityChanged(resource, newQuantity);

        return (resource, removedQuantity);
    }

    public int GetQuantity(ResourceSO resource)
    {
        if (resource == null)
            return 0;

        if (deposit.TryGetValue(resource, out int quantity))
            return quantity;

        return 0;
    }

    private static bool IsValidRequest(ResourceSO resource, int quantity)
    {
        if (resource == null)
        {
            return false;
        }

        if (quantity <= 0)
        {
            return false;
        }

        return true;
    }
}
