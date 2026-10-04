using UnityEngine;

public class MineralNodeView : BuildingView
{
    public MineralNodeData Data => base.Data as MineralNodeData;

    private void Start()
    {
        new MineralNode(Data, this);
    }
}
