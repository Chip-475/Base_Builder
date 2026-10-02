using UnityEngine;

public class MineralNodeView : BuildingView
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public override void Init()
    {

    }

    private void Start()
    {
        new MineralNode(Data, this);
    }
}
