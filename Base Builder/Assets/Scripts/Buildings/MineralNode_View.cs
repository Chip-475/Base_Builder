using UnityEngine;
using static MineralNode;

public class MineralNode_View : Building_View
{
    public MineralNode MineralNode => Building as MineralNode;

    [SerializeField] MineralNodeConfig config = new();

    new void Start()
    {
        Building = new(this, config);
    }
}
