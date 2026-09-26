using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    public Dictionary<Vector3Int, Waypoint> GlobalWaypoints { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }
}
