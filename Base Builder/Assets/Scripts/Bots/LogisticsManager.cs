using UnityEngine;
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
            this.resources = resources;
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
    }

    public static LogisticsManager Instance { get; private set; }
    public static List<LogisticsRequest> Requests { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }
}
