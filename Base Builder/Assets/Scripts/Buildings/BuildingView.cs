using UnityEngine;

public abstract class BuildingView : MonoBehaviour
{
    [field: SerializeField] public BuildingData Data { get; protected set; }
    public Building Obj { get; protected set; }

    protected void OnDrawGizmos()
    {
        // Hitbox gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Data.bounds.size);
    }
}
