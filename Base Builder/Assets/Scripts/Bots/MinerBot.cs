using System.Collections.Generic;
using UnityEngine;

public class MinerBot : Bot
{
    public MinerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    // IWorker
    public override bool CanPerform(WorkTask task)
    {
        if (task.Workplace.GetBuilding() is not MineralNode workplace)
            return Fail(task, "Workplace is not a MineralNode.");
        Pathfinder.Pathfind(task.Worker.GetBot().Coords.ToVector3Int(), workplace.SceneObj.BotWorkPoint.position.ToVector3Int(), out List<Vector3Int> path)
        if (path.Count == 0)
            return Fail(task, "Cannot pathfind to workplace.");
        // inventory check

        return true;
    }
    public override void Perform(WorkTask task)
    {
        throw new System.NotImplementedException();
    }
    public override bool Fail(WorkTask task, string reason)
    {
        throw new System.NotImplementedException();
    }
}
