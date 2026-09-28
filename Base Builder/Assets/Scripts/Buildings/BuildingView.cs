using System;
using UnityEngine;

public abstract class BuildingView : MonoBehaviour
{
    [SerializeField] protected BuildingData Data;

    protected void OnDrawGizmos()
    {
        // Hitbox gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Data.bounds.size);
    }
}
