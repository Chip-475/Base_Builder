using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable
public class Tester : MonoBehaviour
{
    public static Tester Instance { get; private set; }

    public Vector3Int start;
    public Vector3Int end;
    public Sprite cellSprite;

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Trace")]
    public void TracePath()
    {
        var startCell = WorldManager.World.GetCellAt(start);
        var endCell = WorldManager.World.GetCellAt(end);
        Pathfinder.Pathfind(startCell, endCell, out List<Cell> path);

        foreach (var cell in path)
        {
            GameObject go = new();
            go.transform.position = cell.Coords;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = cellSprite;
            sr.color = Color.blue;
        }
    }
}
