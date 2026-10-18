using UnityEngine;
using System.Collections.Generic;

public class MineralNode : Building
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public new MineralNodeView SceneObj => base.SceneObj as MineralNodeView;

    public MineralNode(MineralNodeData data, MineralNodeView sceneObj) : base(data, sceneObj)
    {
        
    }
    public override void Destroy()
    {
        BuildingManager.RegisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj);
    }

    public KeyValuePair<ResourceSO, int> Mine(Bot bot)
    {
        if (!CanMine(bot))
            return new(null, 0);

        return new(Data.resource, Data.baseAmountGiven * (int)Data.purity);
    }
    public bool CanMine(Bot bot)
    {
        KeyValuePair<ResourceSO, int> output = new(Data.resource, Data.baseAmountGiven * (int)Data.purity);

        if (bot.Type != BotType.Miner)
            return false;
        if (bot.MiningLevel < output.Key.toughness)
            return false;
        if (!bot.Inventory.CanAddItem(output.Key, output.Value))
            return false;

        return true;
    }
}
