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
    public Inventory Inv { get; protected set; }
    public float Power { get; protected set; }
    public const float MaxPower = 100;

    public Bot(BotView botView, string id = null, Vector3? coords = null, string name = null, BotType type = BotType.None, Inventory inv = null, float power = MaxPower)
    {
        int maxWeight = type switch
        {
            BotType.Carrier => 100,
            _ => 20
        };

        // Self
        Id = id ?? Guid.NewGuid().ToString();
        Coords = coords ?? Vector3.zero;
        Name = name ?? PickRandomName();
        Type = type;
        Inv = new Inventory(maxWeight) ?? new(inv);
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
public enum BotType
{
    None,
    Miner,
    Carrier,
    Worker,
    Builder,
    Specialist
}
