using UnityEngine;

public class MineralNode : InstalledObject
{
    [Header("Config")]
    [SerializeField] ResourceSO resource;
    [Space]
    [SerializeField] int baseAmountGiven;
    [SerializeField] NodePurity purity;

    public ProcessingData GetProcessingData()
    {
        return new ProcessingData()
        {
            outputResources = new()
            {
                [resource] = baseAmountGiven * (int)purity 
            },
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
