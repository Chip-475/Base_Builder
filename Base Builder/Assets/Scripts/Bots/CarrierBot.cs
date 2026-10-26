using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarrierBot : Bot
{
    public LogisticsManager.LogisticsRequest request;

    bool Working => request != null;

    public CarrierBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    // IWorker
    public override IEnumerator Pipeline()
    {
        while (true)
        {
            if (Working)
            {
                if (Inventory.Contains(request.resources))
                {
                    if (Pathfinder.CanPathfind(Coords.ToVector3Int(), request.position.ToVector3Int()))
                    {
                        if (Vector3.Distance(Coords, request.position) < 0.05f)
                        {
                            request.inventory.AddItems(request.resources);
                            Inventory.RemoveItems(request.resources);
                            ClearRequest(request);

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
                    else
                    {
                        ClearRequest(request);
                        Fail("Cannot pathfind to target.");
                    }
                }
                else
                {
                    if (Depot.Inventory.Contains(request.resources))
                    {
                        if (Pathfinder.CanPathfind(Coords.ToVector3Int(), request.position.ToVector3Int()))
                        {
                            if (Vector3.Distance(Coords, BuildingManager.GetClosestAvailableBuilding<Depot>(Coords.ToVector3Int()).Coords) < BuildingManager.GetClosestAvailableBuilding<Depot>(Coords.ToVector3Int()).Data.Range)
                            {
                                Depot.Inventory.RemoveItems(request.resources);
                                Inventory.AddItems(request.resources);

                                yield return null;
                                continue;
                            }
                            else
                            {
                                SetDestination(BuildingManager.GetClosestAvailableBuilding<Depot>(Coords.ToVector3Int()).Coords);
                                MoveTowardsDestination();

                                yield return null;
                                continue;
                            }
                        }
                        {
                            ClearRequest(request);
                            Fail("Cannot pathfind to target.");
                        }
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                LogisticsManager.LogisticsRequest req = FindAvailableRequest();
                if (req == null)
                {
                    yield return null;
                    continue;
                }
                else
                {
                    OccupyRequest(req);

                    yield return null;
                    continue;
                }
            }
        }
    }

    LogisticsManager.LogisticsRequest FindAvailableRequest()
    {
        foreach (var req in LogisticsManager.Requests)
            if (CanPerformRequest(req))
                return req;

        return null;
    }
    bool CanPerformRequest(LogisticsManager.LogisticsRequest request)
    {
        if (request.taken)
            return false;
        if (!Depot.Inventory.Contains(request.resources))
            return false;
        if (!Inventory.CanAddItems(request.resources))
            return false;

        List<Depot> depots = BuildingManager.GetBuildingsOfType<Depot>().ToList();
        depots.OrderBy(obj => (obj.SceneObj.transform.position - Coords).sqrMagnitude).ToList();
        Depot closest = null;
        foreach (var depot in depots)
        {
            if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), depot.Coords))
                continue;

            closest = depot;
            break;
        }
        if (closest == null)
            return false;

        return true;
    }
    void OccupyRequest(LogisticsManager.LogisticsRequest request)
    {
        request.taken = true;
        this.request = request;
    }
    void ClearRequest(LogisticsManager.LogisticsRequest request)
    {
        LogisticsManager.Requests.Remove(request);
        request = null;
    }
}
