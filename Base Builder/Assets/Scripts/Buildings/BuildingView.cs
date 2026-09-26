using System;
using UnityEngine;

public abstract class BuildingView : MonoBehaviour
{
    [SerializeField] public BuildingData Data;
    public Building Building { get; protected set; }
}
