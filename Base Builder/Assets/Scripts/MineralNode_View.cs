using UnityEngine;
using static MineralNode;

public class MineralNode_View : Building_View
{
    public MineralNode MineralNode { get; private set; }

    [SerializeField] MineralNodeConfig config = new();

    void Start()
    {
        MineralNode = new(this, config);
    }
}
