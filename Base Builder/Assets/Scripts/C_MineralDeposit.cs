using UnityEngine;

public class C_MineralDeposit : MonoBehaviour
{
    private void Start()
    {
        Vector3Int pos = transform.position.ToVector3Int();
        MineralDeposit deposit = new(pos, resource);
        WorldManager.instance.World.GetTileAt(pos).SetDeposit(deposit);
        Destroy(this);
    }

    public ResourceSO resource;
}
