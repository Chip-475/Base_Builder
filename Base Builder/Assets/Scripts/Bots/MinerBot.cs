using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MinerBot : Bot
{
    public MineralNode Node { get; set; }
    public MineralCrate Crate { get; set; }

    public MinerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
        Inventory = new(data.maxWeight);
    }

    // IWorker
    public override async UniTask Pipeline(CancellationToken cancellationToken)
    {
        if (Node == null || Crate == null)
        {
            Fail("MinerBot requires both a mineral node and a crate.");
            return;
        }

        Dictionary<ResourceSO, int> workOutput = Node.GetWorkOutputs();
        if (workOutput == null || workOutput.Count == 0 ||
            workOutput.Any(item => item.Key == null || item.Value <= 0))
        {
            Fail("MinerBot has no valid resource output at its assigned node.");
            return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            if (Inventory.IsEmpty)
            {
                await Mine(workOutput, cancellationToken);
            }
            else
            {
                Deposit(workOutput);
            }

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
    }

    async UniTask Mine(Dictionary<ResourceSO, int> workOutput, CancellationToken cancellationToken)
    {
        if (Node == null || Node.Data == null || !Inventory.CanAddItems(workOutput))
        {
            Fail("MinerBot cannot carry one mining batch.");
            return;
        }

        if (AtWorkPoint(Node))
        {
            await UniTask.Delay(
                System.TimeSpan.FromSeconds(Node.Data.resource.timeToMine),
                cancellationToken: cancellationToken);
            Inventory.AddItems(workOutput);
            return;
        }

        if (!MoveToWorkplace(Node))
            Fail("Cannot pathfind to node.");
    }

    void Deposit(Dictionary<ResourceSO, int> workOutput)
    {
        if (Crate == null || Crate.Inventory == null)
        {
            Fail("MinerBot has no valid mineral crate.");
            return;
        }

        if (!Crate.Inventory.CanAddItems(Inventory.Items))
        {
            if (Crate.Inventory.IsEmpty)
                Fail("Mineral crate cannot accommodate one mining batch.");
            else
                Crate.IssueLogisticsRequest();
            return;
        }

        if (AtWorkPoint(Crate))
        {
            Crate.Inventory.AddItems(Inventory.Items);
            Inventory.Items.Clear();
            destination = null;
            Path.Clear();
            return;
        }

        if (!MoveToWorkplace(Crate))
            Fail("Cannot pathfind to crate.");
    }
}
