using UnityEngine;

[CreateAssetMenu(fileName = "Mineral Node Data", menuName = "Buildings/Mineral Node")]
public class MineralNodeData : BuildingData
{
    public ResourceSO resource;
    public int baseAmountGiven;
    public NodePurity purity;
}
public enum NodePurity
{
    Low = 1,
    Medium = 2,
    High = 3
}
