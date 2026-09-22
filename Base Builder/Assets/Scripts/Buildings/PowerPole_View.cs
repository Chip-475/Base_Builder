using UnityEngine;

public class PowerPole_View : Building_View
{
    public PowerPole PowerPole => Building as PowerPole;

    [SerializeField] PowerPole.Config config;

    new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Bounds bounds = PowerPole.ConnectionBounds;
        Debug.Log(bounds.size);
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));
    }
    private new void Start()
    {
        PoleManager.instance.Start();
        base.Start();
    }
    private void OnDestroy()
    {
        PoleManager.instance.Start();
    }
}
