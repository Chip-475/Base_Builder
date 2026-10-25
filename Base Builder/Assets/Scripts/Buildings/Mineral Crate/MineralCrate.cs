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
    
    public void IssueLogisticsRequest()
    {
        foreach (var request in LogisticsManager.Requests)
            if (request.requester == this)
                return;

        LogisticsManager.Requests.Add(new(this, Inventory.Items, Inventory, BuildingManager.GetClosestAvailableBuilding<Depot>(Coords).Coords));
    }
}
