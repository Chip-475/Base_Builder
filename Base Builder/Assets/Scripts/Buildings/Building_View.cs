using UnityEngine;
using System;
using System.Collections.Generic;

public class Building_View : MonoBehaviour
{
    public Building Building {  get; protected set; }

    protected Building.Config buildingConfig = new();

    protected void OnDrawGizmos()
    {
        Bounds bounds = buildingConfig.bounds;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));
    }
    protected void Start()
    {
        Building = new(this, buildingConfig);
    }
}
