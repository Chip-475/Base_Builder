using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
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
            if (request != null && request.cancelled)
                ReturnCargoAndClearRequest();

            if (!Working)
            {
                if (!Inventory.IsEmpty)
                    TryScheduleRecoveryDelivery();
                else
                {
                    var availableRequest = FindAvailableRequest();
                    if (availableRequest != null)
                        OccupyRequest(availableRequest);
                }
            }
            else if (Inventory.Contains(request.resources))
            {
                AudioManager.PlaySFX(AudioManager.Instance.clips["Bot Carry"], SceneObj.transform);
                DeliverRequest();
            }
            else
            {
                AudioManager.PlaySFX(AudioManager.Instance.clips["Bot Carry"], SceneObj.transform);
                CollectRequest();
            }

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
    }

    void TryScheduleRecoveryDelivery()
    {
        if (Inventory.IsEmpty || Depot.Inventory == null || !Depot.Inventory.CanAddItems(Inventory.Items))
            return;

        Depot closest = null;
        Vector3Int closestAccessPoint = default;
        float closestDistance = float.PositiveInfinity;

        foreach (Depot candidate in BuildingManager.GetBuildingsOfType<Depot>())
        {
            if (!LogisticsManager.TryGetAccessPoint(candidate, Coords, out Vector3Int accessPoint))
                continue;

            float distance = (candidate.Coords - Coords.ToVector3Int()).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            closest = candidate;
            closestAccessPoint = accessPoint;
            closestDistance = distance;
        }

        if (closest == null)
            return;

        request = new LogisticsManager.LogisticsRequest(
            this,
            new System.Collections.Generic.Dictionary<ResourceSO, int>(Inventory.Items),
            Inventory,
            Coords,
            Depot.Inventory,
            closestAccessPoint,
            priority: 1)
        { taken = true };
        pickupPoint = Coords.ToVector3Int();
        deliveryPoint = closestAccessPoint;
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
            && !request.cancelled
            && !request.taken
            && request.sourceInventory != null
            && request.destinationInventory != null
            && request.sourceInventory.Contains(request.resources)
            && (request.sourceInventory == Inventory || Inventory.CanAddItems(request.resources))
            && request.destinationInventory.CanAddItems(request.resources);
    }

    bool TryGetEndpoint(Vector3 position, out Vector3Int endpoint)
    {
        endpoint = position.ToVector3Int();
        Cell cell = WorldManager.World.GetCellAt(endpoint);
        if (cell != null && cell.canWalkOn)
            return true;

        foreach (Building building in BuildingManager.Buildings.Values)
        {
            bool isEndpoint = building.Coords == endpoint
                || building.SceneObj.BotWorkPoint != null
                    && building.SceneObj.BotWorkPoint.position.ToVector3Int() == endpoint;
            if (!isEndpoint)
                continue;

            return LogisticsManager.TryGetAccessPoint(building, Coords, out endpoint);
        }

        return false;
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

        if (!request.sourceInventory.Contains(request.resources)
            || !Inventory.CanAddItems(request.resources))
        {
            FailRequest("The carrier can no longer collect this request.");
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

        ResetRequestState();
    }

    void ClearRequest(bool requeue)
    {
        if (request != null)
        {
            if (requeue && !request.cancelled)
                request.taken = false;
            else
                LogisticsManager.Requests?.Remove(request);
        }

        ResetRequestState();
    }

    void ResetRequestState()
    {

        request = null;
        pickupPoint = null;
        deliveryPoint = null;
        destination = null;
        Path.Clear();
    }

    void FailRequest(string reason)
    {
        ReturnCargoToRequestInventory();
        ClearRequest();
        Debug.LogWarning(reason);
    }

    void ReturnCargoAndClearRequest()
    {
        ReturnCargoToRequestInventory();
        ClearRequest();
    }

    void ReturnCargoToRequestInventory()
    {
        if (request == null || request.sourceInventory == Inventory ||
            !Inventory.Contains(request.resources))
            return;

        if (request.sourceInventory != null && request.sourceInventory.CanAddItems(request.resources))
        {
            request.sourceInventory.AddItems(request.resources);
            Inventory.RemoveItems(request.resources);
        }
        else if (request.destinationInventory != null &&
                 request.destinationInventory.CanAddItems(request.resources))
        {
            request.destinationInventory.AddItems(request.resources);
            Inventory.RemoveItems(request.resources);
        }
    }

    public void PauseForRestore()
    {
        StopWorking();
        if (request == null)
            return;

        ReturnCargoToRequestInventory();
        ClearRequest(requeue: !Inventory.Contains(request.resources));
    }

    public void ResumeAfterLoad()
    {
        if (request == null && !Inventory.IsEmpty && Depot.Inventory != null)
        {
            Depot destination = null;
            Vector3Int accessPoint = default;
            float closestDistance = float.PositiveInfinity;

            foreach (Depot candidate in BuildingManager.GetBuildingsOfType<Depot>())
            {
                if (!Depot.Inventory.CanAddItems(Inventory.Items) ||
                    !LogisticsManager.TryGetAccessPoint(candidate, Coords, out Vector3Int point))
                    continue;

                float distance = (candidate.Coords - Coords.ToVector3Int()).sqrMagnitude;
                if (distance >= closestDistance)
                    continue;

                destination = candidate;
                accessPoint = point;
                closestDistance = distance;
            }

            if (destination != null)
            {
                request = new LogisticsManager.LogisticsRequest(
                    this,
                    new Dictionary<ResourceSO, int>(Inventory.Items),
                    Inventory,
                    Coords,
                    Depot.Inventory,
                    accessPoint,
                    priority: 1)
                { taken = true };
                pickupPoint = Coords.ToVector3Int();
                deliveryPoint = accessPoint;
            }
        }

        if (!Occupied)
            StartWorking();
    }

    public override void Dispose()
    {
        if (request != null)
        {
            if (request.sourceInventory == Inventory && Inventory.Contains(request.resources))
            {
                if (request.destinationInventory != null && request.destinationInventory.CanAddItems(request.resources))
                {
                    request.destinationInventory.AddItems(request.resources);
                    Inventory.RemoveItems(request.resources);
                }
            }
            else
            {
                ReturnCargoToRequestInventory();
            }

            bool requestCanRetry = !request.cancelled && request.sourceInventory != null &&
                request.sourceInventory.Contains(request.resources);
            if (requestCanRetry)
                ClearRequest(requeue: true);
            else
                ClearRequest();
        }

        base.Dispose();
    }
}
