using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class World
{
    [SerializeField] Dictionary<Vector3Int, Cell> cells = new();
    Dictionary<Vector3Int, Building> buildings = new();
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
        WorldBounds = GetWorldBounds();
    }
    public Bounds GetWorldBounds()
    {
        if (cells.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);
        Vector3 min = new(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new(float.MinValue, float.MinValue, float.MinValue);
        foreach (Vector3Int coord in cells.Keys)
        {
            min = Vector3.Min(min, coord);
            max = Vector3.Max(max, coord);
        }
        return new Bounds((min + max) / 2f, (max - min)+new Vector3(1,1,0));
    }
    public Cell GetCellAt(Vector3Int coords) { if (HasCellAt(coords)) return cells[coords]; else return null; }
    public bool HasCellAt(Vector3Int coords) { return cells.ContainsKey(coords); }
    
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
