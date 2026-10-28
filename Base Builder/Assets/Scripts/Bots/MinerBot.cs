using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                Mine(workOutput);
            }
            else
            {
                Deposit(workOutput);
            }

            yield return null;
        }
    }

    void Mine(Dictionary<ResourceSO, int> workOutput)
    {
        if (AtWorkPoint(Node))
        {
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
