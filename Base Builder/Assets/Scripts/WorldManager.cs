using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance {  get; private set; }

    [SerializeField] Grid grid;
    [SerializeField] Dictionary<Tilemap, CellType> mapToType;
    [Space]

    [SerializeField] World world;
    public static World World {  get { return Instance.world; } }

    public Sprite floorSprite;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        grid.gameObject.SetActive(false);
        world = new(grid, mapToType);
    }

    public Cell GetCellUnderMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        return World.GetCellAt(mousePos.ToVector3Int());
    }
}
