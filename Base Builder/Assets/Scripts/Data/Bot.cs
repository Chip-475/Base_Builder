using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Bot
{
    [field: SerializeField]
    public BotView BotView { get; protected set; }

    [Header("Identity")]
    public string Id { get; protected set; }
    public Vector3 Coords {  get; protected set; }
    public string Name { get; protected set; }
    public BotType Type { get; protected set; } = BotType.None;

    [Header("Stats")]
    public float Power { get; protected set; }
    public const float MaxPower = 100;

    public Bot(BotView botView, string id = null, Vector3? coords = null, string name = null, BotType type = BotType.None, float power = MaxPower)
    {
        // Self
        Id = id ?? Guid.NewGuid().ToString();
        Coords = coords ?? Vector3.zero;
        Name = name ?? PickRandomName();
        Type = type;
        Power = power;

        // View
        BotView = botView;
        BotView.name = Name;

        //GameManager.SetBot(Id, this);
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
