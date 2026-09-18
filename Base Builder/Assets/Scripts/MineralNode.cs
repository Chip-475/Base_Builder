using UnityEngine;

public class MineralNode : InstalledObject
{
    [Header("Config")]
    [SerializeField] ResourceSO resource;
    [Space]
    [SerializeField] int baseAmountGiven;
    [SerializeField] NodePurity purity;

    public Cell Cell => WorldManager.World.GetCellAt(transform.position.ToVector3Int());

    public ProcessingData GetProcessingData()
    {
        return new ProcessingData()
        {
            resource = resource,
            amount = baseAmountGiven * (int)purity,
            timeToProcess = resource.toughness * 2
        };
    }
}
public enum NodePurity
{
    Low = 1,
    Medium = 2,
    High = 3
}
