using UnityEngine;

public class Cell
{
    public Cell(Vector2Int coords, CellType type, bool createWorldObject)
    {
        this.coords = coords;
        this.type = type;

        if (createWorldObject)
            CreateWorldObject();
    }

    Vector2Int coords = new(0, 0);
    CellType type = CellType.Void;

    void CreateWorldObject()
    {
        GameObject go = new();
        go.transform.position = coords.ToVector2();
        
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        // assign sprite based on type
    }
}
public enum CellType
{
    Void,
    Floor
}