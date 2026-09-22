using UnityEngine;
using System;
using System.Collections.Generic;

public class Building
{
    [Serializable]
    public class Config
    {
        public Bounds bounds;
        public bool blocksWalking;
        public bool blocksPlacing;
    }

    public Building_View Object { get; private set; }
    public Cell Cell => WorldManager.World.GetCellAt(Object.transform.position.ToVector3Int());
    public Vector3Int Coords => Cell.Coords;

    public Bounds Bounds { get; private set; }
    public bool BlocksWalking { get; private set; }
    public bool BlocksPlacing { get; private set; }

    public Building(Building_View obj, Config config)
    {
        // View
        Object = obj;
        SnapToGrid();
        UpdateCells(GetCellsInBounds(Bounds));

        // Self
        Bounds = config.bounds;
        BlocksWalking = config.blocksWalking;
        BlocksPlacing = config.blocksPlacing;
    }

    void SnapToGrid()
    {
        Object.transform.position = Object.transform.position.ToVector3Int();
    }
    void UpdateCells(Cell[] cells)
    {
        foreach (var cell in cells)
        {
            cell.canWalkOn = !BlocksWalking;
            cell.canBuildOn = !BlocksPlacing;
        }
    }

    public Bounds GetBounds() { return Bounds; }
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

    public void SetPosition(Cell cell)
    {
        Object.transform.position = cell.Coords;
    }
}
