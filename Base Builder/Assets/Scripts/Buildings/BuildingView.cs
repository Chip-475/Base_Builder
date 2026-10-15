using UnityEngine;

public abstract class BuildingView : MonoBehaviour
{
    [field: SerializeField] public BuildingData Data { get; protected set; }
    public Building Obj { get; protected set; }
    [field: SerializeField] public Transform BotWorkPoint { get; protected set; }

    protected void OnDrawGizmos()
    {
        // Hitbox gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Data.bounds.size);

        // Interaction gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Data.interactionBounds.size);

        // Connection gizmo
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Data.connectionBounds.size);
    }
}
