using UnityEngine;

[CreateAssetMenu(fileName = "Path Waypoint Data", menuName = "Buildings/Path Waypoint")]
public class WaypointData : BuildingData
{
    public int range;
    public float movementSpeedModifier = 1f;
}
