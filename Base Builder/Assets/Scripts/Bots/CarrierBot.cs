using System.Collections;
using UnityEngine;

public class CarrierBot : Bot
{
    public LogisticsManager.LogisticsRequest request;
    Depot pickupDepot;
    Vector3Int? destination;

    bool Working => request != null;

    public CarrierBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    // IWorker
    public override IEnumerator Pipeline()
    {
        while (true)
        {
            if (!Working)
            {
                var availableRequest = FindAvailableRequest();
                if (availableRequest != null)
                    OccupyRequest(availableRequest);
            }
            else if (Inventory.Contains(request.resources))
            {
                DeliverRequest();
            }
            else
            {
                CollectRequest();
            }

            yield return null;
        }
    }

    LogisticsManager.LogisticsRequest FindAvailableRequest()
    {
        if (LogisticsManager.Requests == null || LogisticsManager.Requests.Count == 0)
            return null;

        // The depot is independent of the individual request, so find it once
        // instead of recalculating it for every request and every frame.
        pickupDepot = FindClosestReachableDepot();
        if (pickupDepot == null)
            return null;

        foreach (var candidate in LogisticsManager.Requests)
            if (CanPerformRequest(candidate))
                return candidate;

        return null;
    }
    bool CanPerformRequest(LogisticsManager.LogisticsRequest request)
    {
        return request != null
            && !request.taken
            && Depot.Inventory.Contains(request.resources)
            && Inventory.CanAddItems(request.resources);
    }

    Depot FindClosestReachableDepot()
    {
        Vector3Int botCoords = Coords.ToVector3Int();
        Depot closest = null;
        int closestDistance = int.MaxValue;

        foreach (var depot in BuildingManager.GetBuildingsOfType<Depot>())
        {
            if (depot.Occupied || !Pathfinder.CanPathfind(botCoords, depot.Coords))
                continue;

            int distance = (depot.Coords - botCoords).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            closest = depot;
            closestDistance = distance;
        }

        return closest;
    }

    void CollectRequest()
    {
        if (pickupDepot == null || !Depot.Inventory.Contains(request.resources))
        {
            FailRequest("Requested resources are no longer available.");
            return;
        }

        if (!IsInRange(pickupDepot.Coords, pickupDepot.Data.Range))
        {
            if (!MoveTo(pickupDepot.Coords))
                FailRequest("Cannot pathfind to depot.");

            return;
        }

        Depot.Inventory.RemoveItems(request.resources);
        Inventory.AddItems(request.resources);
    }
    void DeliverRequest()
    {
        if (!IsInRange(request.position, 0.05f))
        {
            if (!MoveTo(request.position.ToVector3Int()))
                FailRequest("Cannot pathfind to target.");

            return;
        }

        request.inventory.AddItems(request.resources);
        Inventory.RemoveItems(request.resources);
        ClearRequest();
    }

    bool IsInRange(Vector3 target, float range)
    {
        return (Coords - target).sqrMagnitude < range * range;
    }

    bool MoveTo(Vector3Int target)
    {
        if (destination != target || Path.Count == 0)
        {
            if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), target))
                return false;

            SetDestination(target);
            destination = target;
        }

        MoveTowardsDestination();
        return true;
    }

    void OccupyRequest(LogisticsManager.LogisticsRequest request)
    {
        request.taken = true;
        this.request = request;
    }
    void ClearRequest()
    {
        if (request != null)
            LogisticsManager.Requests?.Remove(request);

        request = null;
        pickupDepot = null;
        destination = null;
        Path.Clear();
    }
    void FailRequest(string reason)
    {
        ClearRequest();
        Fail(reason);
    }
}
