using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Depot : Building, IInteractable
{
    public new DepotData Data => base.Data as DepotData;
    public new DepotView SceneObj => base.SceneObj as DepotView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public static Inventory Inventory { get; private set; }

    public Depot(DepotData data, DepotView sceneObj) : base(data, sceneObj)
    {
        Inventory ??= new(0);
        Inventory?.SetMaxWeight(Inventory.MaxWeight + data.MaxCapacity);
    }
    public override void Destroy()
    {
        Inventory?.SetMaxWeight(Inventory.MaxWeight - Data.MaxCapacity);
        BuildingManager.UnregisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj);
    }
    
    public void StoreItem(ResourceSO resource, int quantity)
    {
        Inventory.AddItem(resource, quantity);
    }
    public void GetItem(ResourceSO resource, int quantity)
    {
        Inventory.RemoveItem(resource, quantity);
    }

    // Work
    public override Dictionary<ResourceSO, int> GetWorkOutputs() => null;

    // Interaction
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
