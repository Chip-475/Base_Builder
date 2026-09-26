using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Waypoint : Building
{
    public new WaypointData Data => base.Data as WaypointData;
    public new WaypointView SceneObj => base.SceneObj as WaypointView;
    
    public Dictionary<Directions, Waypoint> Neighbours { get; private set; } = new()
    {
        [Directions.Up] = null,
        [Directions.Right] = null,
        [Directions.Down] = null,
        [Directions.Left] = null
    };

    public Waypoint(WaypointData data, WaypointView sceneObj) : base(data, sceneObj)
    {
        
    }

    public void SetNeighbour(Directions direction, Waypoint waypoint)
    {
        if (!CanConnectTo(waypoint))
            return;

        Neighbours[direction] = waypoint;
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
    public Directions RelativeDirection(Waypoint waypoint)
    {
        if (!CanConnectTo(waypoint))
            throw new Exception("Waypoints aren't perpendicular.");

        var startCoords = Coords;
        var endCoords = waypoint.Coords;

        return (startCoords, endCoords) switch
        {
            var (start, end) when start.y > end.y => Directions.Up,
            var (start, end) when start.x > end.x => Directions.Right,
            var (start, end) when start.y < end.y => Directions.Down,
            var (start, end) when start.x < end.x => Directions.Left,
            _ => throw new Exception("Waypoints occupy the same cell.")
        };
    }
}
