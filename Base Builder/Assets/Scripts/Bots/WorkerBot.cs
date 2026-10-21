using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WorkerBot : Bot
{
    public WorkerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    // IWorker
    public override bool CanPerform(WorkTask task)
    {

        if (task.Workplace.GetBuilding() is not Machine workplace)
            return Fail(task, "Workplace is not a Machine.");
        if (workplace.Powered)
            return Fail(task, "Workplace is not powered.");
        Pathfinder.Pathfind(task.Worker.GetBot().Coords.ToVector3Int(), workplace.SceneObj.BotWorkPoint.position.ToVector3Int(), out List<Vector3Int> path)
        if (path.Count == 0)
            return Fail(task, "Cannot pathfind to workplace.");
        if (!workplace.Data.usableRecipes.Contains(task.Recipe))
            return Fail(task, "Given recipe is not usable in this workplace.");
        // inventory check
        if (Vector3.Distance(task.Worker.GetBot().Coords, workplace.SceneObj.BotWorkPoint.position) > 0.05f)
            return Fail(task, "Too far from workplace.");

        return true;
    }
    public override void Perform(WorkTask task)
    {
        if (!CanPerform(task))
            return;

        WorkerBot worker = task.Worker.GetBot() as WorkerBot;
        foreach (var kvp in task.Output)
            worker.Inventory.AddItem(kvp.Key, kvp.Value);
    }
    public override bool Fail(WorkTask task, string reason)
    {
        Debug.Log(reason);
        return false;
    }
}
