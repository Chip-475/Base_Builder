using UnityEngine;
using System;
using System.Collections.Generic;

public class PathNetwork
{
    public List<Waypoint> Waypoints { get; private set; } = new();

    public PathNetwork()
    {
        PathingManager.RegisterNetwork(this);
    }

    public void AddWaypoint(Waypoint waypoint)
    {
        if (Waypoints.Contains(waypoint))
            throw new Exception("Waypoint already in network.");

        Waypoints.Add(waypoint);
        waypoint.Network = this;
    }
    public void RemoveWaypoint(Waypoint waypoint)
    {
        if (!Waypoints.Contains(waypoint))
            throw new Exception("Waypoint not in network.");

        Waypoints.Remove(waypoint);
    }

    public void MergeWith(PathNetwork network)
    {
        foreach(var item in network.Waypoints)
            AddWaypoint(item);

        network.Destroy();
    }

    public void Destroy()
    {
        PathingManager.UnregisterNetwork(this);
    }
}
