using System;
using UnityEngine;

public abstract class BuildingView : MonoBehaviour
{
    public BuildingData Data;
    public Building bulding { get; protected set; }
    protected void OnDrawGizmos()
    {
        // Hitbox gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Data.bounds.size);
    }
}
