using UnityEngine;
using System;
using System.Collections.Generic;

public class InstalledObject : MonoBehaviour
{
    [Serializable] public struct ProcessingData
    {
        public ResourceSO resource;
        public int amount;
        public float timeToProcess;
    }

    [SerializeField] protected bool blocksWalking;
    [SerializeField] protected bool blocksPlacing;
    [SerializeField] protected Bounds bounds;
    [SerializeField] protected BoxCollider2D collider;

    void OnValidate()
    {
        collider.size = bounds.size;

        Vector3 worldSize = bounds.size;
        Vector3 localSize = transform.InverseTransformVector(worldSize);
        collider.size = new Vector2
        (
            Mathf.Abs(localSize.x),
            Mathf.Abs(localSize.y)
        );
    }
    void Awake()
    {
        SnapToGrid();
        bounds.center = transform.position;
    }
    void OnDrawGizmos()
    {
        Bounds bounds = this.bounds;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));
    }

    void SnapToGrid()
    {
        transform.position = transform.position.ToVector3Int();
    }

    public Cell[] GetCellsInBounds()
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

    public Cell getCell()
    {
        Vector3Int posCella = transform.position.ToVector3Int(); //da posizione in coordinate
        return WorldManager.World.GetCellAt(posCella); //prende la cella inq quella posizione
    }
}
