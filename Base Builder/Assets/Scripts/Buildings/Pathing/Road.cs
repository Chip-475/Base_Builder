using UnityEngine;

public class Road
{
    public Waypoint StartingPoint { get; private set; }
    public Waypoint EndingPoint { get; private set; }

    public float SpeedModifier =>
        (StartingPoint.Data.movementSpeedModifier + EndingPoint.Data.movementSpeedModifier) / 2f;

    public Road(Waypoint start, Waypoint end)
    {
        StartingPoint = start;
        EndingPoint = end;
    }
}
