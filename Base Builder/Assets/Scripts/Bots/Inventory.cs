using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public Dictionary<ResourceSO, int> Items { get; private set; } = new();
    public int MaxWeight { get; private set; }

    public bool IsEmpty => Items.Count == 0;

    public Inventory(Inventory inv)
    {
        Items = new(inv.Items);
        MaxWeight = inv.MaxWeight;
    }
    public Inventory(int maxWeight)
    {
        MaxWeight = maxWeight;
    }

    public void AddItem(ResourceSO resource, int amount)
    {
        if (!CanAddItem(resource, amount))
        {
            Debug.Log("Cannot add item.");
            return;
        }

        Items[resource] = Items.GetValueOrDefault(resource) + amount;
    }
    public void AddItems(Dictionary<ResourceSO, int> mats)
    {
        if (!CanAddItems(mats))
        {
            Debug.Log("Cannot add items to inventory.");
            return;
        }

        foreach (var mat in mats)
            AddItem(mat.Key, mat.Value);
    }
    public bool CanAddItem(ResourceSO resource, int amount)
    {
        float requestWeight = resource.weightPerUnit * amount;
        float newWeight = requestWeight + GetWeight();
        if (newWeight > MaxWeight)
            return false;

        return true;
    }
    public bool CanAddItems(Dictionary<ResourceSO, int> mats)
    {
        foreach (var mat in mats)
            if (!CanAddItem(mat.Key, mat.Value))
                return false;

        return true;
    }
    
    public KeyValuePair<ResourceSO, int> RemoveItem(ResourceSO resource, int amount)
    {
        if (!CanRemoveItem(resource, amount))
        {
            Debug.Log("Cannot remove item.");
            return new(null, 0);
        }

        Items[resource] = Items.GetValueOrDefault(resource) - amount;
        if (Items[resource] == 0)
            Items.Remove(resource);

        return new(resource, amount);
    }
    public void RemoveItems(Dictionary<ResourceSO, int> mats)
    {
        if (!CanRemoveItems(mats))
        {
            Debug.Log("Cannot remove items to inventory.");
            return;
        }

        foreach (var mat in mats)
            RemoveItem(mat.Key, mat.Value);
    }
    public bool CanRemoveItem(ResourceSO resource, int amount)
    {
        if (!Items.ContainsKey(resource))
            return false;
        if (Items[resource] - amount < 0)
            return false;

        return true;
    }
    public bool CanRemoveItems(Dictionary<ResourceSO, int> mats)
    {
        foreach (var mat in mats)
            if (!CanRemoveItem(mat.Key, mat.Value))
                return false;

        return true;
    }

    public void SetMaxWeight(int amount)
    {
        MaxWeight = amount;
    }

    public float GetWeight()
    {
        float totWeight = 0f;
        foreach (var kvp in Items)
            totWeight += kvp.Key.weightPerUnit * kvp.Value;

        return totWeight;
    }
    public float GetSpecificWeight(ResourceSO resource)
    {
        float weight = 0f;
        foreach (var kvp in Items)
            if (kvp.Key == resource)
                weight += kvp.Key.weightPerUnit * kvp.Value;

        return weight;
    }
    public bool Contains(Dictionary<ResourceSO, int> mats)
    {
        foreach (var mat in mats)
        {
            if (!Items.ContainsKey(mat.Key))
                return false;
            if (Items[mat.Key] < mat.Value)
                return false;
        }

        return true;
    }
}
