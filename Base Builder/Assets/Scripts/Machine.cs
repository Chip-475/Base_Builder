using UnityEngine;
using System.Collections.Generic;

public class Machine : InstalledObject
{
    [Header("Config")]
    [SerializeField] List<RecipeSO> allowedRecipes = new();
    public List<ResourceSO> allowedFuels = new();
    public bool needsEnergy = true;
    public int currentEnergy = 100;
    public int maxEnergy = 100;

    private new void Start()
    {
        base.Start();
        WorldManager.World.SetMachineAt(Coords, this);
    }

    public ProcessingData GetProcessingData()
    {
        return new ProcessingData()
        {
            // To Fill Out
        };
    }
}
