using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEditor.PackageManager;

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;
    public static WorldManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<WorldManager>();
            return _instance;
        }
    }

    [SerializeField] Grid grid;
    [SerializeField] Dictionary<Tilemap, CellType> mapToType;
    [Space]

    [SerializeField] World world;
    bool wordCreato;
    public static World World
    {
        get
        {
            Instance.creaWord();
            return Instance.world;
        }
    }
    void creaWord()
    {
        if (wordCreato) return;
        grid.gameObject.SetActive(false);
        world = new(grid, mapToType);
        wordCreato = true;
    }

    public Sprite floorSprite;

    void Awake()
    {
        _instance = this;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(world.WorldBounds.center, world.WorldBounds.size);
    }
  
}
