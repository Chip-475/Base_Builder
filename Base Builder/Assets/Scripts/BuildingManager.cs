using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

    public static Building GetClosestAvailableBuilding(Vector3Int coords)
    {
        List<Building> buildings = new(Buildings.Values.Where((building) => building.Occupied == false));
        if (buildings.Count == 0)
        {
            Debug.Log("No available buildings.");
            return null;
        }

        Building closest = buildings[0];
        foreach (Building building in buildings)
        {
            float closestDis = Vector3.Distance(closest.Coords, coords);
            float nextDis = Vector3.Distance(building.Coords, coords);

            if (nextDis < closestDis)
                closest = building;
        }

        return closest;
    }
    public static T GetClosestAvailableBuilding<T>(Vector3Int coords)
    where T : Building
    {
        List<T> buildings = Buildings.Values
            .OfType<T>()
            .Where(building => !building.Occupied)
            .ToList();

        if (buildings.Count == 0)
        {
            Debug.Log($"No available buildings of type {typeof(T).Name}.");
            return null;
        }

        T closest = buildings[0];

        foreach (T building in buildings)
        {
            if (!Pathfinder.CanPathfind(coords, building.Coords))
                continue;

            float closestDistance = Vector3.Distance(closest.Coords, coords);
            float buildingDistance = Vector3.Distance(building.Coords, coords);

            if (buildingDistance < closestDistance)
                closest = building;
        }

        return closest;
    }

    public static T[] GetBuildingsOfType<T>()
        where T : Building
    {
        T[] buildings = Buildings.Values.OfType<T>().ToArray();
        return buildings;
    }
}
