using UnityEngine;

public abstract class BuildingView : MonoBehaviour
{
    [field: SerializeField] public BuildingData Data { get; protected set; }
    public Building Obj { get; protected set; }

    protected SpriteRenderer sr;

    protected void Awake()
    {
        TryGetComponent(out BoxCollider2D collider);
        if (collider != null)
            collider.size = Data.bounds.size;

        TryGetComponent(out SpriteRenderer sr);
        this.sr = sr;
    }
    private void OnMouseEnter()
    {
        sr.color = Color.lightGray; 
    }
    private void OnMouseExit()
    {
        sr.color = Color.white;
    }
    protected void OnDrawGizmos()
    {
        // Hitbox gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Data.bounds.size);
    }
}
