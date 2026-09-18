using UnityEngine;
using System;
using System.Collections.Generic;

public class Bot
{
    public struct Inventory
    {
        Dictionary<ResourceSO, int> cargo;
        float maxWeight;

        public void AddResource(ResourceSO resource, int amount)
        {
            cargo ??= new();

            var cargoSim = new Dictionary<ResourceSO, int>(cargo);
            cargoSim[resource] += amount;

            if (cargo[resource] + amount < 0)
            {
                Debug.Log("Error: resource amount cannot be negative.");
                return;
            }
            if(GetTotalWeight(cargoSim) > maxWeight)
            {
                Debug.Log("Error: weight exceeds max capacity.");
                return;
            }

            cargo[resource] += amount;
        }
        public float GetTotalWeight(Dictionary<ResourceSO, int> inv = null)
        {
            inv ??= cargo;

            float totalWeight = 0f;
            foreach (var entry in inv)
                totalWeight += entry.Key.weightPerUnit * entry.Value;

            return totalWeight;
        }
        public float GetSpecificWeight(ResourceSO resource, Dictionary<ResourceSO, int> inv = null)
        {
            inv ??= cargo;

            if (!inv.ContainsKey(resource))
            {
                Debug.Log("Error: resource not present in inventory.");
                return 0f;
            }
            return inv[resource] * resource.weightPerUnit;
        }
    }
    
    public string Id { get; protected set; }
    Vector3Int coords = new(0, 0);
    BotType type = BotType.None;
    public string botName;
    int Power { get; set; }
    int maxPower = 100;
    int maxWeight;

    public Bot(Vector3Int coords, BotType type)
    {
        Id = Guid.NewGuid().ToString();
        this.coords = coords;
        this.type = BotType.None;
        botName = PickRandomName();
        Power = maxPower;
        if (type == BotType.Carrier) maxWeight = 100;
        else maxWeight = 20;
    }

    public string PickRandomName()
    {
        string[] names = { "Alpha", "Bravo", "Charlie", "Delta", "Echo" };
        return names[UnityEngine.Random.Range(0, names.Length)];
    }
}
public enum BotType
{
    None,
    Miner,
    Carrier, //100  altri 20
    Worker,
    Builder,
    Specialist
}
