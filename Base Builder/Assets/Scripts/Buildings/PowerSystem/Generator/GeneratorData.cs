using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Generator Data", menuName = "Buildings/Generator")]
public class GeneratorData : BuildingData
{
    public string id;
    public List<ResourceSO> allowedFuels=new();
}
