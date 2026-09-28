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
    public bool canBuildOn = true;
    public int gCost;
    public int hCost;
    public int F_Cost => gCost + hCost;

    public Cell(Vector3Int coords, CellType type, bool createSceneObject)
    {
        Coords = coords;
        this.type = type;

        if (createSceneObject)
            CreateSceneObject();
    }

    public List<Cell> GetNeighbours()
    {
        List<Cell> cells = new();
        for(int x = -1; x <= 1; x++)
            for(int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0) 
                    continue;

                cells.Add(WorldManager.World.GetCellAt(new Vector3Int(x, y, 0) + Coords));
            }    
        
        return cells;
    }

    void CreateSceneObject()
    {
        GameObject go = new();
        go.transform.SetParent(WorldManager.Instance.transform);
        go.transform.position = Coords.ToVector3();
        go.name = $"Cell_{Coords.x}_{Coords.y}";
        
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Floor";
        sr.sprite = WorldManager.Instance.floorSprite; // Replace when sprite system is established
    }
}
public enum CellType
{
    Void,
    Floor
}