using UnityEngine;

public class MineralDeposit : MonoBehaviour
{
    void OnValidate()
    {
        transform.position = transform.position.ToVector3Int();
        Coords = transform.position.ToVector3Int();
    }

    public Vector3Int Coords {  get; protected set; }
    [field: SerializeField] public ResourceSO Resource { get; protected set; }
}
