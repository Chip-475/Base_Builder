using System.Collections;
using UnityEngine;

public class CarrierBot : Bot
{
    public LogisticsManager.LogisticsRequest request = null;
    Building sourceBuilding;
    Vector3Int? destination;
    Vector3Int? deliveryPoint;

    bool Working => request != null;

    public CarrierBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
        StartWorking();
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

        foreach (var candidate in LogisticsManager.Requests)
        {
            if (!CanPerformRequest(candidate))
                continue;

            Building source = candidate.requester as Building;
            if (!TryFindDeliveryPoint(candidate.position.ToVector3Int(), source, out Vector3Int point))
                continue;

            deliveryPoint = point;
            sourceBuilding = source;
            return candidate;
        }

        return null;
    }
    bool CanPerformRequest(LogisticsManager.LogisticsRequest request)
    {
        return request != null
            && !request.taken
            && request.requester is Building source
            && request.inventory != null
            && request.inventory.Contains(request.resources)
            && Inventory.CanAddItems(request.resources)
            && Pathfinder.CanPathfind(Coords.ToVector3Int(), GetWorkPoint(source).ToVector3Int());
    }

    bool TryFindDeliveryPoint(Vector3Int depotCoords, Building source, out Vector3Int deliveryPoint)
    {
        deliveryPoint = default;
        Depot depot = null;

        foreach (var candidate in BuildingManager.GetBuildingsOfType<Depot>())
        {
            if (candidate.Coords == depotCoords)
            {
                depot = candidate;
                break;
            }
        }

        if (depot == null)
            return false;

        int radius = Mathf.CeilToInt(depot.Data.Range);
        int closestDistance = int.MaxValue;
        Vector3Int sourcePoint = GetWorkPoint(source).ToVector3Int();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int point = depot.Coords + new Vector3Int(x, y, 0);
                Cell cell = WorldManager.World.GetCellAt(point);
                if (cell == null || !cell.canWalkOn)
                    continue;

                if ((point - depot.Coords).sqrMagnitude > depot.Data.Range * depot.Data.Range)
                    continue;

                if (!Pathfinder.CanPathfind(sourcePoint, point))
                    continue;

                int distance = (point - sourcePoint).sqrMagnitude;
                if (distance >= closestDistance)
                    continue;

                deliveryPoint = point;
                closestDistance = distance;
            }
        }

        return closestDistance != int.MaxValue;
    }

    void CollectRequest()
    {
        if (sourceBuilding == null || !request.inventory.Contains(request.resources))
        {
            FailRequest("Requested resources are no longer available.");
            return;
        }

        Vector3 sourcePosition = GetWorkPoint(sourceBuilding);
        if (!IsInRange(sourcePosition, 0.05f))
        {
            if (!MoveTo(sourcePosition.ToVector3Int()))
                FailRequest("Cannot pathfind to source.");

            return;
        }

        request.inventory.RemoveItems(request.resources);
        Inventory.AddItems(request.resources);
    }
    void DeliverRequest()
    {
        if (!deliveryPoint.HasValue)
        {
            FailRequest("Cannot find a delivery point at the depot.");
            return;
        }

        if (!IsInRange(deliveryPoint.Value, 0.05f))
        {
            if (!MoveTo(deliveryPoint.Value))
                FailRequest("Cannot pathfind to target.");

            return;
        }

        if (!Depot.Inventory.CanAddItems(request.resources))
        {
            FailRequest("Depot cannot accommodate the requested resources.");
            return;
        }

        Depot.Inventory.AddItems(request.resources);
        Inventory.RemoveItems(request.resources);
        ClearRequest();
    }

    Vector3 GetWorkPoint(Building building)
    {
        return building.SceneObj.BotWorkPoint != null
            ? building.SceneObj.BotWorkPoint.position
            : building.Coords;
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
        sourceBuilding = null;
        destination = null;
        deliveryPoint = null;
        Path.Clear();
    }
    void FailRequest(string reason)
    {
        ClearRequest();
        Fail(reason);
    }
}
