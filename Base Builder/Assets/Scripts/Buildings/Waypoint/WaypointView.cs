using UnityEngine;

public class WaypointView : BuildingView
{
    public new WaypointData Data => base.Data as WaypointData;

    private void Start()
    {
        new Waypoint(Data, this);
    }
    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.white;
        Bounds[] range = new Bounds[2];
        range[0] = new(transform.position, new Vector3(1, Data.range * 2 + 1, 0));
        range[1] = new(transform.position, new Vector3(Data.range * 2 + 1, 1, 0));

        foreach (var item in range)
            Gizmos.DrawWireCube(transform.position, item.size);
    }
}
