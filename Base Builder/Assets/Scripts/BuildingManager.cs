using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public static Dictionary<Vector3Int, Building> Buildings { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }

    public static void RegisterBuilding(Building building)
    {
        if (Buildings.ContainsKey(building.Coords))
        {
            Debug.Log($"Building already registered at {building.Coords}");
            return;
        }

        Buildings.Add(building.Coords, building);
    }
    public static void UnregisterBuilding(Building building)
    {
        if (!Buildings.ContainsKey(building.Coords))
        {
            Debug.Log($"No building registered at {building.Coords}");
            return;
        }

        Buildings.Remove(building.Coords);
    }
    
    public Building GetBuildingAt(Vector3Int position)
    {
        if(Buildings.TryGetValue(position, out var building))
            return building;

        Debug.Log($"No building found at {position}");
        return null;
    }
    public Building GetBuildingAt(Vector3Int position, Type type)
    {
        if (Buildings.TryGetValue(position, out var building) && building.GetType() == type)
            return building;

        Debug.Log($"No building of type {type} found at {position}");
        return null;
    }
}
