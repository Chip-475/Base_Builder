using UnityEngine;

public class MineralNode : InstalledObject
{
    [Header("Config")]
    [SerializeField] ResourceSO resource;
    [SerializeField] int amountPerProcess;
    [SerializeField] NodePurity purity;

    private new void Start()
    {
        base.Start();
        WorldManager.World.SetMineralNodeAt(Coords, this);
    }

    public ProcessingData GetProcessingData()
    {
        return new ProcessingData()
        {
            outputResources = new()
            {
                [resource] = amountPerProcess * (int)purity 
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
