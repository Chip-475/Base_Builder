using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class World
{
    public Dictionary<Vector3Int, Cell> Cells { get; private set; } = new();
    Dictionary<Vector3Int, Building> buildings = new();
    public Bounds WorldBounds;

    public World(Grid grid, Dictionary<Tilemap, CellType> mapToType)
    {
        foreach (Tilemap tilemap in grid.GetComponentsInChildren<Tilemap>())
            foreach (Vector3Int coord in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(coord))
                    continue;

                Cells[coord] = new(coord, mapToType[tilemap], true);
            }
        WorldBounds = GetWorldBounds();
    }
    public Bounds GetWorldBounds()
    {
        if (Cells.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);
        Vector3 min = new(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new(float.MinValue, float.MinValue, float.MinValue);
        foreach (Vector3Int coord in Cells.Keys)
        {
            min = Vector3.Min(min, coord);
            max = Vector3.Max(max, coord);
        }
        return new Bounds((min + max) / 2f, (max - min)+new Vector3(1,1,0));
    }
    public Cell GetCellAt(Vector3Int coords) { if (HasCellAt(coords)) return Cells[coords]; else return null; }
    public bool HasCellAt(Vector3Int coords)
    {
        //if(Cells==null) return false;
        return Cells.ContainsKey(coords);
    }

    public void RegisterBuilding(Vector3Int coords, Building building)
    {
        if (!buildings.ContainsKey(coords))
            buildings.Add(coords, building);
        else
            Debug.Log("Cannot register building.");
    }
    public void UnregisterBuilding(Vector3Int coords)
    {
        if (!buildings.ContainsKey(coords))
            Debug.Log($"No building found at {coords}");
        else
            buildings.Remove(coords);
    }
}
