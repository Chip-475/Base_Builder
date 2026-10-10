using System;
using Unity.Collections;
using UnityEngine;

public class Depot : Building
{
    public new DepotData Data => base.Data as DepotData;
    public new DepotView SceneObj => base.SceneObj as DepotView;

    private static PlayerInventory sharedInventory;

    public Depot(DepotData data, DepotView sceneObj) : base(data, sceneObj)
    {
        sharedInventory ??= new();
    }
    public override void Destroy()
    {
        WorldManager.World.UnregisterBuilding(Coords);
        MonoBehaviour.Destroy(SceneObj);
    }
    
    public void StoreItem(ResourceSO resource, int quantity)
    {
        sharedInventory.Add(resource, quantity);
    }
    public void GetItem(ResourceSO resource, int quantity)
    {
        sharedInventory.Remove(resource, quantity);
    }
}
