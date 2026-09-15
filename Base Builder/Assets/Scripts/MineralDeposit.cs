using UnityEngine;

public class MineralDeposit : MonoBehaviour
{
    void OnValidate()
    {
        Vector2 pos = transform.position;
        Vector2Int newPos = pos.ToVector2Int();
        transform.position = newPos.ToVector2();
    }
}
