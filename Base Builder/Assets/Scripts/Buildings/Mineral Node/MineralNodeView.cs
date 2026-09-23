using UnityEngine;

public class MineralNodeView : BuildingView
{
    public MineralNodeData Data => data as MineralNodeData;

    private void Start()
    {
        new MineralNode(Data, this);
    }
}
