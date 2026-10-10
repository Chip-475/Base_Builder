using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class Building
{
    public BuildingData Data { get; private set; }
    public BuildingView SceneObj { get; private set; }

    public Vector3Int Coords => SceneObj.transform.position.ToVector3Int();
    public Bounds Bounds { get; private set; }
    public Bounds ConnectionBounds { get; private set; }
    public NetworkManager Network; 

    public Building(BuildingData data, BuildingView sceneObj)
    {
        Data = data;
        SceneObj = sceneObj;

        //WorldManager.World.RegisterBuilding(this, Coords);
        Bounds = new(Coords, Data.bounds.size);
        ConnectionBounds = new(Coords, Data.connectionBounds.size);
        UpdateCells(GetCellsInBounds(GetBounds()));
        PowerManager.instance.RegisterBuilding();
    }
    public abstract void Destroy();

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
            cell.canWalkOn = !Data.blocksWalking;
            cell.canBuildOn = !Data.blocksPlacing;
        }
    }
    protected void FreeUpCells(Cell[] cells)
    {
        foreach(var cell in cells)
        {
            cell.canWalkOn = true;
            cell.canBuildOn = true;
        }
    }
}
