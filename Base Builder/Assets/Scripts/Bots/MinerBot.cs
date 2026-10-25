using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinerBot : Bot
{
    public MineralNode Node { get; set; }
    public MineralCrate Crate { get; set; } // To change to Crate once it is created

    public MinerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    // IWorker
    public override IEnumerator Pipeline()
    {
        Dictionary<ResourceSO, int> workOutput = Node.GetWorkOutputs();

        while (true)
        {
            if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), Node.Coords))
                Fail("Cannot pathfind to node.");
            if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), Crate.Coords))
                Fail("Cannot pathfind to crate.");

            if (Inventory.IsEmpty)
            {
                if (IsInWorkRange(Node))
                {
                    // Work
                    yield return null;
                    continue;
                }
                else
                {
                    SetDestination(Node.Coords);
                    MoveTowardsDestination();
                    yield return null;
                    continue;
                }
            }
            else
            {
                if (Crate.Inventory.CanAddItems(workOutput))
                {
                    if (IsInWorkRange(Crate))
                    {
                        Crate.Inventory.AddItems(Inventory.Items);
                        Inventory.Items.Clear();
                        yield return null;
                        continue;
                    }
                    else
                    {
                        SetDestination(Crate.Coords);
                        MoveTowardsDestination();
                        yield return null;
                        continue;
                    }
                }
                else
                {
                    Fail("Crate cannot accomodate items.");
                }
            }

            yield return null;
        }
    }
}
