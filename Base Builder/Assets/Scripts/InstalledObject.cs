using UnityEngine;
using System;
using System.Collections.Generic;

public class InstalledObject : MonoBehaviour
{
    [Serializable]
    public struct ProcessingData
    {
        [SerializeField] public Dictionary<ResourceSO, int> inputResources;
        [SerializeField] public Dictionary<ResourceSO, int> outputResources;
        public float timeToProcess;
    }

    [SerializeField] protected bool blocksWalking;
    [SerializeField] protected bool blocksPlacing;
    [SerializeField] protected Bounds bounds;
    [SerializeField] protected BoxCollider2D collider;

    [field: SerializeField] public int Power { get; protected set; }
    public Cell Cell => WorldManager.World.GetCellAt(transform.position.ToVector3Int());
    public Vector3Int Coords => Cell.Coords;


    public void OnDrawGizmos()
    {
        Bounds bounds = this.bounds;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));
    }
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
    protected void Start()
    {
        UpdateCells(GetCellsInBounds(bounds));
    }

    void SnapToGrid()
    {
        transform.position = transform.position.ToVector3Int();
    }
    void UpdateCells(Cell[] cells)
    {
        foreach (var cell in cells)
        {
            cell.canWalkOn = !blocksWalking;
            cell.canBuildOn = !blocksPlacing;
        }
    }

    public Cell[] GetCellsInBounds(Bounds bounds)
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

    public Bounds GetBounds() { return bounds; }

    public void SetColor(Color color)
    {
        if (!TryGetComponent(out SpriteRenderer sr))
            return;

        sr.color = color;
    }
    public void SetPosition(Cell cell)
    {
        gameObject.transform.position = cell.Coords;
    }

}
