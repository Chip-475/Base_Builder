using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DepotData", menuName = "Buildings/Depot")]

public class DepotData : BuildingData
{
    //Size se serve fare avere la grandezza del deposito
    public int MaxCapacity=5;
}