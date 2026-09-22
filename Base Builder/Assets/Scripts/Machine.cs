using UnityEngine;
using System.Collections.Generic;

public class Machine : InstalledObject
{
    [Header("Config")]
    [SerializeField] List<RecipeSO> allowedRecipes = new();
    public List<ResourceSO> allowedFuels = new();
    private bool isworking;
    private bool isWorking { 
        get { return isworking; } 
        set
        {
            isworking=value;
            if(isworking)poleManager.instance.powerConsumption+=energyConsumption;
            else poleManager.instance.powerConsumption-=energyConsumption;
        } 
    }
    public bool needsEnergy = true;
    public int energyConsumption = 1;//to decide if energyConsuption is per machine or per recipe
    /*[HideInInspector]*/public bool isPowered = false;
    /*[HideInInspector]*/public bool isConnected = false;

    private new void Start()
    {
        base.Start();
        WorldManager.World.SetMachineAt(Coords, this);
    }
    public void checkConnection()
    {
        foreach (PowerPole pole in poleManager.instance.powerPoles)
        {
                if (pole.CanConnectTo(this))
                {
                    pole.Connect(this);
                }
        }
    }
    public ProcessingData GetProcessingData()
    {
        return new ProcessingData()
        {
            // To Fill Out
        };
    }
}
