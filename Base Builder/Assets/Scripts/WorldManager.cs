using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [SerializeField] Grid grid;
    [SerializeField] Dictionary<Tilemap, CellType> mapToType;
    [Space]

    [SerializeField] World world;

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
}
