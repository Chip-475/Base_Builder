using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bot Data", menuName = "Bots/Bot Data")]
public class BotData : ScriptableObject
{
    public string ID;
    public Sprite sprite;
    [Space]
    [SerializeField] public Dictionary<ResourceSO, int> cost = new();
    public float speed;
    public int maxWeight;
    public float powerConsumption;
    public int maxPower;
    public int carryCapacity;
    public WorkerBot.Type type;
}