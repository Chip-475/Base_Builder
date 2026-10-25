using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CarrierBot : Bot
{
    public LogisticsManager.LogisticsRequest request;

    public CarrierBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    // IWorker
    public override IEnumerator Pipeline()
    {
        while (true)
        {
            if (request == null)
            {
                // Request searching
                foreach (var req in LogisticsManager.Requests)
                {
                    if (req.taken)
                        continue;
                    if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), req.position.ToVector3Int())
                        && !Inventory.CanAddItems(req.resources))
                        continue;

                    request = req;
                    break;
                }

                yield return null;
                continue;
            }
            else
            {
                if (Inventory.Contains(request.resources))
                {
                    if (Vector3.Distance(Coords, request.position) < 0.05f)
                    {
                        request.inventory.AddItems(request.resources);
                        Inventory.Items.Clear();

                        yield return null;
                        continue;
                    }
                    else
                    {
                        SetDestination(request.position.ToVector3Int());
                        MoveTowardsDestination();

                        yield return null;
                        continue;
                    }
                }
                else // Doesnt have specified resources
                {
                    Depot closest = BuildingManager.GetClosestAvailableBuilding<Depot>(Coords.ToVector3Int());

                    if (Vector3.Distance(Coords, closest.Coords) < 0.05f)
                    {
                        Inventory.AddItems(request.resources);
                        Depot.Inventory.RemoveItems(request.resources);

                        yield return null;
                        continue;
                    }
                    else
                    {
                        SetDestination(closest.Coords);
                        MoveTowardsDestination();

                        yield return null;
                        continue;
                    }
                }
            }
        }
    }
}
