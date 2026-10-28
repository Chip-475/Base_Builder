using UnityEngine;
using System.Collections.Generic;

public class LogisticsManager : MonoBehaviour
{
    public class LogisticsRequest
    {
        public LogisticsRequest(object requester, Dictionary<ResourceSO, int> resources, Inventory inventory, Vector3 position)
        {
            this.requester = requester;
            this.resources = resources;
            this.inventory = inventory;
            this.position = position;
        }

        public object requester;

        public Dictionary<ResourceSO, int> resources = new();
        public Inventory inventory;
        public Vector3 position;

        public bool taken;
    }

    public static LogisticsManager Instance { get; private set; }
    public static List<LogisticsRequest> Requests { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }
}
