using UnityEngine;
using System;
using System.Collections.Generic;

public class PathingManager : MonoBehaviour
{
    public static PathingManager Instance { get; private set; }

    public Dictionary<Vector3Int, Waypoint> AllWaypoints { get; private set; } = new();
    public List<PathNetwork> Networks { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Connect Waypoints")]
    public void ConnectWaypoints()
    {
        foreach(var item in AllWaypoints.Values)
        {
            var connectables = item.GetConnectables();
            foreach(var connectable in connectables.Values)
            {
                if(connectable != null) item.ConnectTo(connectable);
            }
        }
    }

    public static void RegisterWaypoint(Waypoint waypoint)
    {
        if (Instance.AllWaypoints.TryGetValue(waypoint.Coords, out _))
            throw new Exception("Waypoint already exists.");

        Instance.AllWaypoints[waypoint.Coords] = waypoint;
    }
    public static void UnregisterWaypoint(Waypoint waypoint)
    {
        if (!Instance.AllWaypoints.TryGetValue(waypoint.Coords, out _))
            throw new Exception("Waypoint doesn't exist.");

        Instance.AllWaypoints[waypoint.Coords] = null;
    }

    public static void RegisterNetwork(PathNetwork network)
    {
        if (Instance.Networks.Contains(network))
            throw new Exception("Network is already registered.");

        Instance.Networks.Add(network);
    }
    public static void UnregisterNetwork(PathNetwork network)
    {
        if (!Instance.Networks.Contains(network))
            throw new Exception("Network is not registered.");

        Instance.Networks.Remove(network);
    }
}
