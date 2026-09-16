using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

[Serializable]
public class World
{
    [SerializeField] Dictionary<Vector3Int, Cell> cells = new();

    public World(Grid grid, Dictionary<Tilemap, CellType> mapToType)
    {
        foreach (Tilemap tilemap in grid.GetComponentsInChildren<Tilemap>())
            foreach (Vector3Int coord in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(coord))
                    continue;

                cells[coord] = new(coord, mapToType[tilemap], true);
            }
    }

    public Cell GetCellAt(Vector3Int coords) { return cells[coords]; }
}
