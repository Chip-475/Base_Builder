using System.Collections.Generic;
using UnityEngine;

public abstract class Building : IWorkplace
{ 
    public BuildingData Data { get; private set; }
    public BuildingView SceneObj { get; private set; }

    public Vector3Int Coords => SceneObj.transform.position.ToVector3Int();
    public Bounds Bounds { get; set; }
    public Bounds ConnectionBounds { get; set; }
    public Bounds InteractionBounds 
    {  
        get
        {
            return new Bounds(Coords, Data.interactionBounds.size);
        }
        //private set; 
    }
    public NetworkManager Network;
    public bool Occupied { get; set; }
    public bool IsConnected => Network != null;

    public Building(BuildingData data, BuildingView sceneObj)
    {
        Data = data;
        SceneObj = sceneObj;

        Bounds = new(Data.bounds.center + Coords, Data.bounds.size);
        //InteractionBounds = new(Data.interactionBounds.center + Coords, Data.interactionBounds.size);
        ConnectionBounds = new(Data.connectionBounds.center + Coords, Data.connectionBounds.size);
        UpdateCells(GetCellsInBounds(GetBounds()));
        BuildingManager.RegisterBuilding(this);
        PowerManager.instance.RegisterBuilding();
    }
    public abstract void Destroy();

    // IWorkplace
    public Building GetBuilding() => this;
    public abstract Dictionary<ResourceSO, int> GetWorkOutputs();

    // Getters - Setters
    public Bounds GetBounds() { return Bounds; }
    public Bounds GetConnectionBounds() { return ConnectionBounds; }
    public Cell[] GetCellsInBounds(Bounds bounds)
    {
        int minX = Mathf.CeilToInt(bounds.min.x);
        int maxX = Mathf.FloorToInt(bounds.max.x);
        int minY = Mathf.CeilToInt(bounds.min.y);
        int maxY = Mathf.FloorToInt(bounds.max.y);

        List<Cell> cells = new();
        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                Cell cell = WorldManager.World.GetCellAt(new Vector3Int(x, y, 0));
                if (cell == null) continue;
                cells.Add(cell);
            }

        return cells.ToArray();
    }


    protected void UpdateCells(Cell[] cells)
    {
        foreach(var cell in cells)
        {
            if(SceneObj.BotWorkPoint != null && cell == WorldManager.World.GetCellAt(SceneObj.BotWorkPoint.position.ToVector3Int()))
                cell.canWalkOn = true;
            else
                cell.canWalkOn = !Data.blocksWalking;

            cell.canBuildOn = !Data.blocksPlacing;
            cell.Building = this;
        }
    }
    protected void FreeUpCells(Cell[] cells)
    {
        foreach(var cell in cells)
        {
            cell.canWalkOn = true;
            cell.canBuildOn = true;
            cell.Building = null;
        }
    }
}
