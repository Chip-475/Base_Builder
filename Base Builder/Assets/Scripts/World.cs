using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

public class World
{
    public World(Grid grid)
    {
        foreach (Tilemap tilemap in grid.GetComponentsInChildren<Tilemap>())
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos))
                    continue;

                Vector2Int coord = pos.ToVector2Int();
                cells[coord] = new(coord, CellType.Floor, true);
            }
    }

    Dictionary<Vector2Int, Cell> cells = new();
}
