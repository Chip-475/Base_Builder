using System;
using UnityEngine;

public class DepotView : BuildingView
{
    public new DepotData Data => base.Data as DepotData;

    public event Action<ResourceSO, int> InventoryChanged;

    private void Start()
    {
        new Depot(Data, this);
    }
}
