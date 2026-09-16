using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class InstalledObject
{
    public bool blocksWalking;
    public bool blocksPlacing;
    public Bounds size;

    public Cell Cell { get; protected set; }
    public Bounds Size { get; protected set; }

    public InstalledObject(Cell cell, Bounds size)
    {
        Cell = cell;
        Size = size;
        this.size.center = size.center;

        cell.AddInstalledObject(this);
    }

    public Cell[] GetCellsInBounds()
    {
        int minX = Mathf.CeilToInt(Size.min.x);
        int maxX = Mathf.FloorToInt(Size.max.x);
        int minY = Mathf.CeilToInt(Size.min.y);
        int maxY = Mathf.FloorToInt(Size.max.y);

        List<Cell> cells = new();
        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                Cell cell = WorldManager.instance.World.GetCellAt(new Vector3Int(x, y, 0));
                cells.Add(cell);
            }

        return cells.ToArray();
    }
}
