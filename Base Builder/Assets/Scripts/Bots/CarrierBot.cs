using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CarrierBot : Bot
{
    public LogisticsManager.LogisticsRequest request;

    Vector3Int? pickupPoint;
    Vector3Int? deliveryPoint;

    bool Working => request != null;

    public CarrierBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
        StartWorking();
    }

    public override async UniTask Pipeline(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
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

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
    }

    LogisticsManager.LogisticsRequest FindAvailableRequest()
    {
        if (LogisticsManager.Requests == null || LogisticsManager.Requests.Count == 0)
            return null;

        LogisticsManager.LogisticsRequest bestRequest = null;
        Vector3Int bestPickupPoint = default;
        Vector3Int bestDeliveryPoint = default;
        int bestPriority = int.MinValue;
        int bestDistance = int.MaxValue;

        foreach (var candidate in LogisticsManager.Requests)
        {
            if (!CanPerformRequest(candidate))
                continue;

            if (!TryGetEndpoint(candidate.sourcePosition, out Vector3Int sourcePoint)
                || !TryGetEndpoint(candidate.destinationPosition, out Vector3Int targetPoint))
                continue;

            if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), sourcePoint)
                || !Pathfinder.CanPathfind(sourcePoint, targetPoint))
                continue;

            int distance = (sourcePoint - Coords.ToVector3Int()).sqrMagnitude;
            if (candidate.Priority < bestPriority
                || candidate.Priority == bestPriority && distance >= bestDistance)
                continue;

            bestRequest = candidate;
            bestPickupPoint = sourcePoint;
            bestDeliveryPoint = targetPoint;
            bestPriority = candidate.Priority;
            bestDistance = distance;
        }

        if (bestRequest == null)
            return null;

        pickupPoint = bestPickupPoint;
        deliveryPoint = bestDeliveryPoint;
        return bestRequest;
    }

    bool CanPerformRequest(LogisticsManager.LogisticsRequest request)
    {
        return request != null
            && !request.taken
            && request.sourceInventory != null
            && request.destinationInventory != null
            && request.sourceInventory.Contains(request.resources)
            && Inventory.CanAddItems(request.resources)
            && request.destinationInventory.CanAddItems(request.resources);
    }

    bool TryGetEndpoint(Vector3 position, out Vector3Int endpoint)
    {
        endpoint = position.ToVector3Int();
        Cell cell = WorldManager.World.GetCellAt(endpoint);
        if (cell != null && cell.canWalkOn)
            return true;

        Depot depot = FindDepotAt(endpoint);
        if (depot == null)
            return false;

        int radius = Mathf.CeilToInt(depot.Data.Range);
        int closestDistance = int.MaxValue;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int candidate = depot.Coords + new Vector3Int(x, y, 0);
                Cell candidateCell = WorldManager.World.GetCellAt(candidate);
                if (candidateCell == null || !candidateCell.canWalkOn)
                    continue;

                if ((candidate - depot.Coords).sqrMagnitude > depot.Data.Range * depot.Data.Range)
                    continue;

                int distance = (candidate - depot.Coords).sqrMagnitude;
                if (distance >= closestDistance)
                    continue;

                endpoint = candidate;
                closestDistance = distance;
            }
        }

        return closestDistance != int.MaxValue;
    }

    Depot FindDepotAt(Vector3Int coords)
    {
        foreach (var depot in BuildingManager.GetBuildingsOfType<Depot>())
            if (depot.Coords == coords)
                return depot;

        return null;
    }

    void CollectRequest()
    {
        if (!pickupPoint.HasValue || !request.sourceInventory.Contains(request.resources))
        {
            FailRequest("Requested resources are no longer available.");
            return;
        }

        if (!IsInRange(pickupPoint.Value))
        {
            if (!MoveTo(pickupPoint.Value))
                FailRequest("Cannot pathfind to resource source.");

            return;
        }

        request.sourceInventory.RemoveItems(request.resources);
        Inventory.AddItems(request.resources);
    }

    void DeliverRequest()
    {
        if (!deliveryPoint.HasValue)
        {
            FailRequest("Cannot find a logistics destination.");
            return;
        }

        if (!IsInRange(deliveryPoint.Value))
        {
            if (!MoveTo(deliveryPoint.Value))
                FailRequest("Cannot pathfind to logistics destination.");

            return;
        }

        if (!request.destinationInventory.CanAddItems(request.resources))
        {
            FailRequest("Logistics destination cannot accommodate the resources.");
            return;
        }

        request.destinationInventory.AddItems(request.resources);
        Inventory.RemoveItems(request.resources);
        ClearRequest();
    }

    bool IsInRange(Vector3Int target)
    {
        return (Coords - target).sqrMagnitude < 1f * 1f;
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
        pickupPoint = null;
        deliveryPoint = null;
        destination = null;
        Path.Clear();
    }

    void FailRequest(string reason)
    {
        ClearRequest();
        Fail(reason);
    }
}
