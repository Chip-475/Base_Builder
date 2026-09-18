using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : InstalledObject
{
    [Header("Config")]
    [SerializeField] List<RecipeSO> allowedRecipes = new();
    public List<ResourceSO> allowedFuels = new();
    public bool needsEnergy = true;
    public int currentEnergy = 100;
    public int maxEnergy = 100;

    public ProcessingData GetProcessingData()
    {
        return new ProcessingData()
        {
            // To Fill Out
        };
    }
}
