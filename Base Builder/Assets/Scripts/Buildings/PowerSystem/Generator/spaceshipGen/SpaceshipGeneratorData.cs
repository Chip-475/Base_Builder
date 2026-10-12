using UnityEngine;
[CreateAssetMenu(fileName = "Spaceship Generator Data", menuName = "Buildings/Spaceship Generator")]
public class SpaceshipGeneratorData : BuildingData
{
    public string id;
    public ResourceSO[] allowedFuels;
    public float fuelConsumptionRate;
    public float productionRate;
}
