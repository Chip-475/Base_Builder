using UnityEngine;
using System;

public class Depot : Building
{
     public new DepotData Data => base.Data as DepotData;
    public new DepotView SceneObj => base.SceneObj as DepotView;

public Depot(MineralNodeData data, MineralNodeView sceneObj) : base(data, sceneObj)
    {
        
    }
}
