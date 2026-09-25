using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Bot
{
    [Serializable] public struct Inventory
    {
        public Dictionary<ResourceSO, int> Cargo {  get; private set; }
        public float MaxWeight {  get; private set; }
        public Inventory(Inventory? inv = null, int? maxWeight = null)
        {
            Cargo = inv?.Cargo ?? new();
            MaxWeight = maxWeight.Value;
        }

        public void AddResource(ResourceSO resource, int amount)
        {
            Cargo ??= new();

            var cargoSim = new Dictionary<ResourceSO, int>(Cargo);
            cargoSim[resource] += amount;

            if (Cargo[resource] + amount < 0)
            {
                Debug.Log("Error: resource amount cannot be negative.");
                return;
            }
            if(GetTotalWeight(cargoSim) > MaxWeight)
            {
                Debug.Log("Error: weight exceeds max capacity.");
                return;
            }

            Cargo[resource] += amount;
        }

        public readonly float GetTotalWeight(Dictionary<ResourceSO, int> inv = null)
        {
            inv ??= Cargo;

            float totalWeight = 0f;
            foreach (var entry in inv)
                totalWeight += entry.Key.weightPerUnit * entry.Value;

            return totalWeight;
        }
        public readonly float GetSpecificWeight(ResourceSO resource, Dictionary<ResourceSO, int> inv = null)
        {
            inv ??= Cargo;

            if (!inv.ContainsKey(resource))
            {
                Debug.Log("Error: resource not present in inventory.");
                return 0f;
            }
            return inv[resource] * resource.weightPerUnit;
        }

        public void SetMaxWeight(float newMax) { MaxWeight = newMax; }
    }

    [field: SerializeField]
    public Bot_View BotView { get; protected set; }

    [Header("Identity")]
    public string Id { get; protected set; }
    public Vector3 Coords {  get; protected set; }
    public string Name { get; protected set; }
    public BotType Type { get; protected set; } = BotType.None;

    [Header("Stats")]
    public Inventory Inv { get; protected set; }
    public float Power { get; protected set; }
    public const float MaxPower = 100;

    public Bot(Bot_View botView, string id = null, Vector3? coords = null, string name = null, BotType type = BotType.None, Inventory? inv = null, float power = MaxPower)
    {
        // Self
        Id = id ?? Guid.NewGuid().ToString();
        Coords = coords ?? Vector3.zero;
        Name = name ?? PickRandomName();
        Type = type;
        Inv = inv ?? new();
        Power = power;

        // View
        BotView = botView;
        BotView.name = Name;

        GameManager.SetBot(Id, this);
    }

    string PickRandomName()
    {
        string[] names_1 = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett" };
        string[] names_2 = { "Leader", "Keeper", "Pioneer", "Witcher", "Diver", "Bomber", "Rancher", "Taker", "Dispatcher", "Manager" };
        return names_1[UnityEngine.Random.Range(0, names_1.Length)] + " " + names_2[UnityEngine.Random.Range(0, names_2.Length)];
    }
}
public enum BotType
{
    None,
    Miner,
    Carrier,
    Worker,
    Builder,
    Specialist
}
