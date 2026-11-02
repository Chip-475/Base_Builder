using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Generator Data", menuName = "Buildings/Generator")]
public class GeneratorData : BuildingData
{
    public string id;
    public List<ResourceSO> allowedFuels=new();
    public ResourceSO defFuel;
    public int defFuelAmount;
    public bool isSpaceshipGen=false;
    private void OnValidate()
    {
        if (!allowedFuels.Contains(defFuel)) { defFuel = null; defFuelAmount = 0; }
    }
}
