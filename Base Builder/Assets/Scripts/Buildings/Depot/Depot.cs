using System;
using Unity.Collections;
using UnityEngine;

public class Depot : Building, IInteractable
{
    public new DepotData Data => base.Data as DepotData;
    public new DepotView SceneObj => base.SceneObj as DepotView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public static PlayerInventory Inventory { get; private set; }

    public Depot(DepotData data, DepotView sceneObj) : base(data, sceneObj)
    {
        Inventory ??= new();
    }
    public override void Destroy()
    {
        BuildingManager.UnregisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj);
    }
    
    public void StoreItem(ResourceSO resource, int quantity)
    {
        Inventory.Add(resource, quantity);
    }
    public void GetItem(ResourceSO resource, int quantity)
    {
        Inventory.Remove(resource, quantity);
    }

    public bool CanInteractWith()
    {
        var cells = GetCellsInBounds(Data.interactionBounds);
        foreach (var item in cells)
            if (item == PlayerManager.Instance.GetPlayerCell()) return true;

        return false;
    }
    public void Interact()
    {
        // open UI
    }
}
