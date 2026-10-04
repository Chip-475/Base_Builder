using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class World
{
    [SerializeField] Dictionary<Vector3Int, Cell> cells = new();
    [SerializeField] Dictionary<Vector3Int, MachineView> machines = new();
    public Bounds WorldBounds;

    public World(Grid grid, Dictionary<Tilemap, CellType> mapToType)
    {
        foreach (Tilemap tilemap in grid.GetComponentsInChildren<Tilemap>())
            foreach (Vector3Int coord in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(coord))
                    continue;

                cells[coord] = new(coord, mapToType[tilemap], true);
            }
        WorldBounds = getWorldBounds();
    }
    public Bounds getWorldBounds()
    {
        if (cells.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        foreach (Vector3Int coord in cells.Keys)
        {
            min = Vector3.Min(min, coord);
            max = Vector3.Max(max, coord);
        }
        return new Bounds((min + max) / 2f, (max - min)+new Vector3(1,1,0));
    }
    public Cell GetCellAt(Vector3Int coords) { return cells[coords]; }
    public bool HasCellAt(Vector3Int coords) { return cells.ContainsKey(coords); }
    public MachineView GetMachineAt(Vector3Int coords) { return machines[coords]; }

    public void SetMachineAt(Vector3Int coords, MachineView machine) { machines[coords] = machine; }
}
