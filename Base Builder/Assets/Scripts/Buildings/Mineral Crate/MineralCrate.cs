using System.Collections.Generic;
using UnityEngine;

public class MineralCrate : Building
{
    public new MineralCrateData Data => base.Data as MineralCrateData;
    public new MineralCrateView SceneObj => base.SceneObj as MineralCrateView;

    public Inventory Inventory { get; set; }

    public MineralCrate(BuildingData data, BuildingView sceneObj) : base(data, sceneObj)
    {
        Inventory = new(Data.maxWeight);
    }
    public override void Destroy()
    {
        BuildingManager.RegisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj);
    }
    public override Dictionary<ResourceSO, int> GetWorkOutputs() => null;

    public void AddItem(ResourceSO resource, int amount) => Inventory.AddItem(resource, amount);
    public void AddItems(Dictionary<ResourceSO, int> mats) => Inventory.AddItems(mats);

    public bool CanAddItem(ResourceSO resource, int amount) => Inventory.CanAddItem(resource, amount);
    public bool CanAddItems(Dictionary<ResourceSO, int> mats) => Inventory.CanAddItems(mats);

    public KeyValuePair<ResourceSO, int> RemoveItem(ResourceSO resource, int amount) => Inventory.RemoveItem(resource, amount);
    public bool CanRemoveItem(ResourceSO resource, int amount) => Inventory.CanRemoveItem(resource, amount);
}
