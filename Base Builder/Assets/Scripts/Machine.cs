using UnityEngine;
using System.Collections.Generic;

public class Machine : InstalledObject
{
    [Header("Config")]
    public List<RecipeSO> allowedRecipes = new();
    public List<ResourceSO> allowedFuels = new();
    public bool needsEnergy = true;
    public int currentEnergy = 100;
    public int maxEnergy = 100;

    private new void Start()
    {
        base.Start();
        WorldManager.World.SetMachineAt(Coords, this);
    }
    void OnMouseDown()
    {
        if (machineRecipeUI.instance != null) machineRecipeUI.instance.apri(this);
    }
    public ProcessingData GetProcessingData(RecipeSO recipe)
    {
        ProcessingData dati = new ProcessingData();
        dati.inputResources = new Dictionary<ResourceSO, int>();
        dati.outputResources = new Dictionary<ResourceSO, int>();
        dati.timeToProcess = recipe.completionTime;
        for(int i=0;i<recipe.inputResources.Length;i++)
        {
            ResourceSO ris = recipe.inputResources[i];
            int quant = recipe.input[i];
            dati.inputResources[ris] = quant;
        }
        for(int i=0;i<recipe.output.Length;i++)
        {
            ResourceSO ris = recipe.outputResources[i];
            int quant = recipe.output[i];
            dati.outputResources[ris] = quant;
        }
        return dati;
    }
}
