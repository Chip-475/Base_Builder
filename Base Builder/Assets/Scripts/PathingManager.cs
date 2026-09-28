using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;

public class PathingManager : MonoBehaviour
{
    public static PathingManager Instance { get; private set; }

    public Dictionary<Vector3Int, Waypoint> AllWaypoints { get; private set; } = new();
    public List<PathNetwork> Networks { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }

    public void ConnectWaypoints(Waypoint a, Waypoint b)
    {
        if(a == null || b == null)
        {
            Debug.Log("One or more waypoints are nulll.");
            return;
        }
        if (a.IsConnectedTo(b))
        {
            Debug.Log("Waypoints are already connected.");
            return;
        }

        a.ConnectTo(b);
        b.ConnectTo(a);

        if (a.Network != b.Network)
            a.Network.MergeWith(b.Network);
    }
    public void DisconnectWaypoints(Waypoint a, Waypoint b)
    {
        if (a == null || b == null)
        {
            Debug.Log("One or more waypoints are nulll.");
            return;
        }
        if (!a.IsConnectedTo(b))
        {
            Debug.Log("Waypoints are already disconnected.");
            return;
        }

        a.DisconnectFrom(b);
        b.DisconnectFrom(a);
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

        Instance.AllWaypoints.Remove(waypoint.Coords);
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
