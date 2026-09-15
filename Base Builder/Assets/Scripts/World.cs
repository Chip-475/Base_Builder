using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

[Serializable]
public class World
{
    public World(Grid grid, Dictionary<Tilemap, CellType> mapToType)
    {
        foreach (Tilemap tilemap in grid.GetComponentsInChildren<Tilemap>())
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos))
                    continue;

                Vector2Int coord = pos.ToVector2Int();
                cells[coord] = new(coord, mapToType[tilemap], true);
            }
    }

    [SerializeField] Dictionary<Vector2Int, Cell> cells = new();

    public Cell GetTileAt(Vector2Int coords) { return cells[coords]; }
}
