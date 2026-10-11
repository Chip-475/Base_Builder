using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Spaceship Data", menuName = "Buildings/Spaceship")]
public class SpaceshipData : BuildingData
{
    public float powerGeneration;
    [SerializeField] public Dictionary<BotType, BotView> botPrefabs = new();
}