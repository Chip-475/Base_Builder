using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class MinerBot : Bot
{
    public MineralNode Node { get; set; }
    public MineralCrate Crate { get; set; }

    public MinerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
        Inventory = new(999);
    }

    // IWorker
    public override IEnumerator Pipeline()
    {
        if (Node == null || Crate == null)
        {
            Fail("MinerBot requires both a mineral node and a crate.");
            yield break;
        }

        Dictionary<ResourceSO, int> workOutput = Node.GetWorkOutputs();

        while (true)
        {
            if (Inventory.IsEmpty)
            {
                await Mine(workOutput);
            }
            else
            {
                Deposit(workOutput);
            }

            yield return null;
        }
    }

   async UniTask Mine(Dictionary<ResourceSO, int> workOutput)
    {
        if (AtWorkPoint(Node))
        {
            await UniTask.WaitForSeconds(Node.Data.resource.timeToMine);
            Inventory.AddItems(workOutput);
            return;
        }

        if (!MoveToWorkplace(Node))
            Fail("Cannot pathfind to node.");
    }

    void Deposit(Dictionary<ResourceSO, int> workOutput)
    {
        if (!Crate.Inventory.CanAddItems(workOutput))
        {
            Fail("Crate cannot accommodate items.");
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
