using UnityEngine;

public class MineralNode : Building
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public new MineralNodeView SceneObj => base.SceneObj as MineralNodeView;

    public MineralNode(MineralNodeData data, MineralNodeView sceneObj) : base(data, sceneObj)
    {
        // fill out as necessary
    }
}
