using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public static Dictionary<Vector3Int, Building> Buildings { get; private set; } = new();
    public static Dictionary<string, HashSet<Building>> BuildingsByID { get; private set; } = new();
    public static HashSet<string> UnlockedBuildingIDs { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }

    public static void RegisterBuilding(Building building)
    {
        if (building == null)
            return;

        if (string.IsNullOrWhiteSpace(building.ID))
        {
            Debug.LogError($"Cannot register building '{building.GetType().Name}': its ID is empty.");
            return;
        }

        if (Buildings.ContainsKey(building.Coords))
        {
            Debug.Log($"Building already registered at {building.Coords}");
            return;
        }

        Buildings.Add(building.Coords, building);

        if (!BuildingsByID.TryGetValue(building.ID, out HashSet<Building> buildings))
        {
            buildings = new();
            BuildingsByID.Add(building.ID, buildings);
        }

        buildings.Add(building);
    }
    public static void UnregisterBuilding(Building building)
    {
        if (building == null)
            return;

        if (!Buildings.TryGetValue(building.Coords, out Building registeredBuilding) ||
            registeredBuilding != building)
        {
            Debug.Log($"No building registered at {building.Coords}");
        }
        else
        {
            Buildings.Remove(building.Coords);
        }

        if (BuildingsByID.TryGetValue(building.ID, out HashSet<Building> buildings))
        {
            buildings.Remove(building);

            if (buildings.Count == 0)
                BuildingsByID.Remove(building.ID);
        }
    }

    public static Building[] GetBuildingsByID(string buildingID)
    {
        if (string.IsNullOrWhiteSpace(buildingID) ||
            !BuildingsByID.TryGetValue(buildingID, out HashSet<Building> buildings))
            return Array.Empty<Building>();

        return buildings.ToArray();
    }

    public static bool IsBuildingUnlocked(string buildingID)
    {
        return !string.IsNullOrWhiteSpace(buildingID) &&
            UnlockedBuildingIDs.Contains(buildingID);
    }

    public static void UnlockBuilding(string buildingID)
    {
        if (!string.IsNullOrWhiteSpace(buildingID))
            UnlockedBuildingIDs.Add(buildingID);
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
