using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [SerializeField] Grid grid;
    [SerializeField] Dictionary<Tilemap, CellType> mapToType;
    [Space]

    [SerializeField] World world;
    public static World World {  get { return instance.world; } }

    public Sprite floorSprite;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        grid.gameObject.SetActive(false);
        world = new(grid, mapToType);
    }
    public Cell GetCellCoordsFromMouse()
    {
        Mouse mouse = Mouse.current;
        Vector3 mousePos=Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        mousePos.z = 0;
        return World.GetCellAt(mousePos.ToVector3Int());
    }
}
