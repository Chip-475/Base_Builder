using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Bot
{
    public BotView BotView { get; protected set; }
    public BotData BotData { get; protected set; }

    [Header("Runtime Data")]
    public string Id { get; protected set; }
    public Vector3 Coords {  get; protected set; }
    public string Name { get; protected set; }
    public float Power { get; protected set; }

    public Bot(BotView view, BotData data, string id = null, Vector3? coords = null, string name = null, float power = 100)
    {
        BotView = view;
        BotData = data;

        // Self
        Id = id ?? Guid.NewGuid().ToString();
        Coords = coords ?? Vector3.zero;
        Name = name ?? PickRandomName();
        Power = power;

        // View
        BotView.name = Name;

        Tester.Instance.bot = this;
    }

    public void MoveTo(Vector3Int coords)
    {
        BotView.SetDestination(coords);
        BotView.StartMoving();
    }

    string PickRandomName()
    {
        string[] names_1 = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett" };
        string[] names_2 = { "Leader", "Keeper", "Pioneer", "Witcher", "Diver", "Bomber", "Rancher", "Taker", "Dispatcher", "Trickster" };
        return names_1[UnityEngine.Random.Range(0, names_1.Length)] + " " + names_2[UnityEngine.Random.Range(0, names_2.Length)];
    }
}
public class BotInventory
{
    public Dictionary<ResourceSO, int> Inventory { get; private set; } = new();
    public int MaxWeight { get; private set; }

    public BotInventory(BotInventory inv)
    {
        Inventory = new(inv.Inventory);
        MaxWeight = inv.MaxWeight;
    }
    public BotInventory(int maxWeight)
    {
        MaxWeight = maxWeight;
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
