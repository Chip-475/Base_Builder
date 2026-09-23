using System.Collections.Generic;
using UnityEngine;

public abstract class Building
{
    public BuildingData Data { get; private set; }
    public BuildingView SceneObj { get; private set; }

    public Vector3Int Coords { get; private set; }

    public Building(BuildingData data, BuildingView sceneObj)
    {
        Data = data;
        SceneObj = sceneObj;
    }


    // Getters - Setters
    public Bounds GetBounds() { return Data.bounds; }
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
                cells.Add(cell);
            }

        return cells.ToArray();
    }

    public void SetPosition(Vector3Int coords)
    {
        SceneObj.transform.position = coords;
    }
}
