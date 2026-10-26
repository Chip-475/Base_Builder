using UnityEngine;
using UnityEngine.Rendering;

public class MineralNodeView : BuildingView
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public new MineralNode RuntimeObj
    {
        get { return base.RuntimeObj as MineralNode; }
        set { base.RuntimeObj = value; }
    }

    private void Start()
    {
        RuntimeObj = new MineralNode(Data, this);
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked");

        var bot = BotManager.GetClosestAvailableBot<MinerBot>(RuntimeObj);
        bot.Node = RuntimeObj;
        bot.Crate = BuildingManager.GetClosestAvailableBuilding<MineralCrate>(RuntimeObj.Coords);
        bot.StartWorking();
    }
}
