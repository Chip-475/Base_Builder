using UnityEngine;
[CreateAssetMenu(fileName = "Generator Data", menuName = "Buildings/Generator")]
public class GeneratorData : BuildingData
{
    public string id;
    public ResourceSO[] allowedFuels;
    public float fuelConsumptionRate;
    public float productionRate;
}
