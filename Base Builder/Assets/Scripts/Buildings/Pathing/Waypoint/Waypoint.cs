using UnityEngine;
using System;
using System.Collections.Generic;

public class Waypoint : Building
{
    public new WaypointData Data => base.Data as WaypointData;
    public new WaypointView SceneObj => base.SceneObj as WaypointView;

    public PathNetwork Network { get; set; }

    public Dictionary<Directions, Waypoint> Neighbours { get; private set; } = new()
    {
        [Directions.Up] = null,
        [Directions.Right] = null,
        [Directions.Down] = null,
        [Directions.Left] = null
    };

    public Waypoint(WaypointData data, WaypointView sceneObj) : base(data, sceneObj)
    {
        PathingManager.RegisterWaypoint(this);
    }
    public override void Destroy()
    {
        foreach (var item in Neighbours.Values)
            if(item != null) DisconnectFrom(item);
        Network.RemoveWaypoint(this);
        PathingManager.UnregisterWaypoint(this);

        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public void ConnectTo(Waypoint target)
    {
        if (IsConnectedTo(target))
            throw new Exception("Waypoint is already connected to target.");
        if (!CanConnectTo(target))
            throw new Exception("Waypoint is out of range.");

        Neighbours[RelativeDirection(target)] = target;
    }
    public void DisconnectFrom(Waypoint target)
    {
        if (!IsConnectedTo(target))
            return;

        var direction = RelativeDirection(target);
        if (Neighbours[direction] == null)
            throw new Exception("Waypoint is not connected to target.");

        Neighbours[direction] = null;
    }
    

    public bool CanConnectTo(Waypoint waypoint)
    {
        var startCoords = Coords;
        var endCoords = waypoint.Coords;

        if (startCoords.x != endCoords.x && startCoords.y != endCoords.y)
            return false;
        if (Vector3Int.Distance(startCoords, endCoords) > Data.range)
            return false;

        return true;
    }
    public bool IsConnectedTo(Waypoint waypoint)
    {
        if (waypoint == null)
            throw new Exception("Waypoint is null.");

        foreach(var item in Neighbours.Values)
            if (item == waypoint) return true;

        return false;
    }
    public Directions RelativeDirection(Waypoint waypoint)
    {
        var startCoords = Coords;
        var endCoords = waypoint.Coords;

        if (startCoords.x != endCoords.x && startCoords.y != endCoords.y)
            throw new Exception("Waypoints aren't perpendicular.");

        return (startCoords, endCoords) switch
        {
            var (start, end) when start.y < end.y => Directions.Up,
            var (start, end) when start.x < end.x => Directions.Right,
            var (start, end) when start.y > end.y => Directions.Down,
            var (start, end) when start.x > end.x => Directions.Left,
            _ => throw new Exception("Waypoints occupy the same cell.")
        };
    }
}
