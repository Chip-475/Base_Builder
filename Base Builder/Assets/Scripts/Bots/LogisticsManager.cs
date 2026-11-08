using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LogisticsManager : MonoBehaviour
{
    public class LogisticsRequest
    {
        public LogisticsRequest(
            object requester,
            Dictionary<ResourceSO, int> resources,
            Inventory sourceInventory,
            Vector3 sourcePosition,
            Inventory destinationInventory,
            Vector3 destinationPosition,
            int priority = 0)
        {
            this.requester = requester;
            this.resources = resources != null
                ? new Dictionary<ResourceSO, int>(resources)
                : new Dictionary<ResourceSO, int>();
            this.sourceInventory = sourceInventory;
            this.sourcePosition = sourcePosition;
            this.destinationInventory = destinationInventory;
            this.destinationPosition = destinationPosition;
            Priority = priority;
        }

        public object requester;

        public Dictionary<ResourceSO, int> resources = new();
        public Inventory sourceInventory;
        public Vector3 sourcePosition;
        public Inventory destinationInventory;
        public Vector3 destinationPosition;
        public int Priority { get; set; }

        public bool taken;
        public bool cancelled;
    }

    public static LogisticsManager Instance { get; private set; }
    public static List<LogisticsRequest> Requests { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
        Requests.Clear();
    }

    public static void CancelRequest(LogisticsRequest request)
    {
        if (request == null)
            return;

        request.cancelled = true;
        Requests?.Remove(request);
    }

    public static bool TryGetAccessPoint(Building building, Vector3 from, out Vector3Int accessPoint)
    {
        accessPoint = default;
        if (building == null || building.SceneObj == null || WorldManager.World == null)
            return false;

        if (building.SceneObj.BotWorkPoint != null)
        {
            Vector3Int workPoint = building.SceneObj.BotWorkPoint.position.ToVector3Int();
            Cell workCell = WorldManager.World.GetCellAt(workPoint);
            if (workCell != null && workCell.canWalkOn &&
                Pathfinder.CanPathfind(from.ToVector3Int(), workPoint))
            {
                accessPoint = workPoint;
                return true;
            }
        }

        float maxDistance = building is Depot depot
            ? Mathf.Max(1f, depot.Data.Range)
            : Mathf.Max(1.5f, Mathf.Max(building.Bounds.extents.x, building.Bounds.extents.y) + 1.25f);
        float maxDistanceSqr = maxDistance * maxDistance;
        float closestDistance = float.PositiveInfinity;

        foreach (Cell cell in WorldManager.World.Cells.Values)
        {
            if (cell == null || !cell.canWalkOn)
                continue;

            Vector3 candidatePosition = cell.Coords;
            Vector3 closestBoundsPoint = building.Bounds.ClosestPoint(candidatePosition);
            if ((candidatePosition - closestBoundsPoint).sqrMagnitude > 1.1f * 1.1f)
                continue;

            if (building is Depot && (candidatePosition - building.Coords).sqrMagnitude > maxDistanceSqr)
                continue;

            float distance = (candidatePosition - from).sqrMagnitude;
            if (distance >= closestDistance || !Pathfinder.CanPathfind(from.ToVector3Int(), cell.Coords))
                continue;

            accessPoint = cell.Coords;
            closestDistance = distance;
        }

        return !float.IsPositiveInfinity(closestDistance);
    }

    public static float LargestCarrierCapacity => BotManager.Bots
        .OfType<CarrierBot>()
        .Where(bot => bot != null)
        .Select(bot => bot.Inventory.MaxWeight)
        .DefaultIfEmpty(0)
        .Max();

    public static List<Dictionary<ResourceSO, int>> BuildLoads(
        Dictionary<ResourceSO, int> resources,
        float capacity)
    {
        if (resources == null || capacity <= 0)
            return null;

        List<Dictionary<ResourceSO, int>> loads = new();
        Dictionary<ResourceSO, int> currentLoad = new();
        float remaining = capacity;

        foreach (var item in resources)
        {
            if (item.Key == null || item.Value <= 0 || item.Key.weightPerUnit < 0)
                return null;

            float unitWeight = item.Key.weightPerUnit;
            int remainingAmount = item.Value;
            while (remainingAmount > 0)
            {
                int amount = unitWeight <= 0
                    ? remainingAmount
                    : Mathf.Min(remainingAmount, Mathf.FloorToInt(remaining / unitWeight));

                if (amount <= 0)
                {
                    if (currentLoad.Count == 0)
                        return null;

                    loads.Add(currentLoad);
                    currentLoad = new();
                    remaining = capacity;
                    continue;
                }

                currentLoad[item.Key] = currentLoad.GetValueOrDefault(item.Key) + amount;
                remainingAmount -= amount;
                if (unitWeight > 0)
                    remaining -= unitWeight * amount;

                if (remainingAmount > 0)
                {
                    loads.Add(currentLoad);
                    currentLoad = new();
                    remaining = capacity;
                }
            }
        }

        if (currentLoad.Count > 0)
            loads.Add(currentLoad);

        return loads;
    }
}
