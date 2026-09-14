using UnityEngine;
public enum BotType
{
    none,
    transporter,
    builder,
    miner,
    worker,
    specialist
}
public class Bot
{
    public BotType type;
    public string id;
    public string name;
    public Vector2Int cellCoords;
    public int maxPower;
    public int currentPower;
    public Bot()
    {
        type = BotType.none;
        name = pickRandomName();
        cellCoords = new Vector2Int();
        maxPower = 100;
        id=System.Guid.NewGuid().ToString();
    }
    public string pickRandomName()
    {
        string[] names = { "Alpha", "Bravo", "Charlie", "Delta", "Echo" };
        return names[Random.Range(0, names.Length)];
    }
}
