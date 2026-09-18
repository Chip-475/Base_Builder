using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Cell
{
    public Vector3Int Coords{ get; protected set; }
    CellType type = CellType.Void;
    public CellType Type 
    {  
        get { return type; }
        protected set { type = value; }
    }

    public bool canWalkOn = true;
    public bool canPlaceOn = true;

    public Cell(Vector3Int coords, CellType type, bool createSceneObject)
    {
        Coords = coords;
        this.type = type;

        if (createSceneObject)
            CreateSceneObject();
    }

    void CreateSceneObject()
    {
        GameObject go = new();
        go.transform.position = Coords.ToVector3();
        go.name = $"Cell_{Coords.x}_{Coords.y}";
        
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Floor";
        sr.sprite = WorldManager.instance.floorSprite; // Replace when sprite system is established
    }
}
public enum CellType
{
    Void,
    Floor
}