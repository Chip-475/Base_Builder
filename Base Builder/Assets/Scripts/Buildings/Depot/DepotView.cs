using System;
using UnityEngine;

public class DepotView : BuildingView
{
    public DepotData Data => base.Data as DepotData;
    public Depot Building { get; private set; }

    public event Action<ResourceSO, int> InventoryChanged;

    private void Start()
    {
        if (Data == null)
        {
            enabled = false;
            return;
        }

        Building = new Depot(Data, this);
        Building.InventoryChanged += ChangedInventory;
    }

    private void OnDestroy()
    {
        if (Building != null)
            Building.InventoryChanged -= ChangedInventory;
    }

    private void ChangedInventory(ResourceSO resource, int quantity)
    {
        if (InventoryChanged != null)
        {
            InventoryChanged(resource, quantity);
        }
    }
}
