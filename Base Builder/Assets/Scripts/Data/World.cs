using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

[Serializable]
public class World
{
    [SerializeField] Dictionary<Vector3Int, Cell> cells = new();
    [SerializeField] Dictionary<Vector3Int, MachineView> machines = new();

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
    public MachineView GetMachineAt(Vector3Int coords) { return machines[coords]; }

    public void SetMachineAt(Vector3Int coords, MachineView machine) {  machines[coords] = machine; }
}
