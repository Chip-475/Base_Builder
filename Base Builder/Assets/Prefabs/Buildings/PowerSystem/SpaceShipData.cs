using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Spaceship Data", menuName = "Buildings/Spaceship")]
public class SpaceshipData : BuildingData
{
    public float powerGeneration;
    public List<BotView> botPrefabs = new();
}