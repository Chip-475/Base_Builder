using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Machine Data", menuName = "Buildings/Machine")]
public class MachineData : BuildingData
{
    public List<RecipeSO> usableRecipes;
}
