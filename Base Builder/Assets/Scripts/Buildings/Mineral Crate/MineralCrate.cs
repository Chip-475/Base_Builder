using System.Collections.Generic;
using UnityEngine;

public class MineralCrate : Building
{
    public new MineralCrateData Data => base.Data as MineralCrateData;
    public new MineralCrateView SceneObj => base.SceneObj as MineralCrateView;

    public Inventory Inventory { get; set; }
    LogisticsManager.LogisticsRequest activeRequest;
    bool disposed;

    public MineralCrate(BuildingData data, BuildingView sceneObj) : base(data, sceneObj)
    {
        Inventory = new(Data.maxWeight);
    }
    public override void Destroy()
    {
        Dispose();
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        LogisticsManager.CancelRequest(activeRequest);
        FreeUpCells(GetCellsInBounds(GetBounds()));
        BuildingManager.UnregisterBuilding(this);
        if (PowerManager.instance != null)
            PowerManager.instance.UnregisterBuilding();
    }
    public override Dictionary<ResourceSO, int> GetWorkOutputs() => null;
    
    public void IssueLogisticsRequest()
    {
        if (Inventory == null || Inventory.IsEmpty || Depot.Inventory == null)
            return;

        if (activeRequest != null && LogisticsManager.Requests.Contains(activeRequest))
            return;
        activeRequest = null;

        List<Dictionary<ResourceSO, int>> loads = LogisticsManager.BuildLoads(
            Inventory.Items, LogisticsManager.LargestCarrierCapacity);
        if (loads == null || loads.Count == 0)
            return;

        Dictionary<ResourceSO, int> load = loads[0];

        Depot destination = null;
        float closestDistance = float.PositiveInfinity;
        foreach (Depot depot in BuildingManager.GetBuildingsOfType<Depot>())
        {
            if (!Depot.Inventory.CanAddItems(load) ||
                !LogisticsManager.TryGetAccessPoint(depot, Coords, out _))
                continue;

            float distance = (depot.Coords - Coords).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            destination = depot;
            closestDistance = distance;
        }

        if (destination == null)
            return;

        activeRequest = new LogisticsManager.LogisticsRequest(
            this,
            load,
            Inventory,
            SceneObj.BotWorkPoint != null ? SceneObj.BotWorkPoint.position : Coords,
            Depot.Inventory,
            destination.SceneObj.BotWorkPoint != null ? destination.SceneObj.BotWorkPoint.position : destination.Coords);
        LogisticsManager.Requests.Add(activeRequest);
    }
}
