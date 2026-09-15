using UnityEngine;
using System;

[Serializable]
public class Cell
{
    public Cell(Vector3Int coords, CellType type, bool createWorldObject)
    {
        this.coords = coords;
        this.type = type;

        if (createWorldObject)
            CreateWorldObject();
    }

    Vector3Int coords = new(0, 0);
    CellType type = CellType.Void;
    Action cellTypeChanged;

    public CellType Type 
    {  
        get { return type; }
        set { type = value; cellTypeChanged(); }
    }


    void CreateWorldObject()
    {
        GameObject go = new();
        go.transform.position = coords.ToVector3();
        go.name = $"Cell_{coords.x}_{coords.y}";
        
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Floor";
        sr.sprite = WorldManager.instance.floorSprite; // Replace when sprite system is established
        cellTypeChanged += () => ChangeSprite(sr);
    }
    void ChangeSprite(SpriteRenderer sr)
    {
        // select sprite based on type
    }
}
public enum CellType
{
    Void,
    Floor
}