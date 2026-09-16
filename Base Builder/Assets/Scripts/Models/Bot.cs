using UnityEngine;
using System;

public class Bot
{
    public Bot(Vector2Int coords, BotType type)
    {
        Id = Guid.NewGuid().ToString();

        this.coords = coords;
        this.type = BotType.None;
        name = PickRandomName();
        Power = maxPower;
    }

    public string Id { get; protected set; }

    Vector2Int coords = new(0, 0);
    BotType type = BotType.None;
    public string name;

    int Power { get; set; }
    int maxPower = 100;
    
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
    Carrier,
    Worker,
    Builder,
    Specialist
}
