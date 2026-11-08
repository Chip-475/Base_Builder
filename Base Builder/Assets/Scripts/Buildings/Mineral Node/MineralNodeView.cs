using UnityEngine;
using System.Linq;
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
        InitializeNode();
    }

    private void OnMouseDown()
    {
        InitializeNode();
        if (RuntimeObj == null)
            return;

        if (Helpers.IsMouseOverUI())
            return;

        var bot = BotManager.GetClosestAvailableBot<MinerBot>(RuntimeObj);
        if (bot == null)
        {
            Debug.LogWarning("No available miner bot.");
            return;
        }

        MineralCrate crate = BuildingManager.GetBuildingsOfType<MineralCrate>()
            .Where(candidate => candidate != null &&
                LogisticsManager.TryGetAccessPoint(candidate, bot.Coords, out _))
            .OrderBy(candidate => (candidate.Coords - bot.Coords.ToVector3Int()).sqrMagnitude)
            .FirstOrDefault();
        if (crate == null)
        {
            Debug.LogWarning("No reachable mineral crate.");
            return;
        }

        bot.Node = RuntimeObj;
        bot.Crate = crate;
        bot.StartWorking();
    }

    void InitializeNode()
    {
        if (RuntimeObj != null)
            return;

        if (Data == null)
        {
            Debug.LogError($"Mineral node '{name}' has no MineralNodeData assigned.", this);
            return;
        }

        RuntimeObj = new MineralNode(Data, this);
    }

    private void OnDestroy()
    {
        RuntimeObj?.Dispose();
    }
}
