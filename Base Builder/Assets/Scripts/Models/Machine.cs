using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : InstalledObject
{
    //lista di ricette che la macchina può eseguire,
    // prendono un imput e tornano un output facendo lavorare il bot per x tempo 
    public List<RecipeSO> allowedRecipe = new();


    [Header("Energy Management")]
    public bool needsEnergy = true;
    public List<ResourceSO> allowedFuels = new();
    public int currentEnergy;
    public int maxEnergy;
    //coal value=50 

    [Header("Identity")]
    public string id;
    public string m_name;
    [TextArea] public string m_desc;
    public Sprite m_sprite;

    public Vector3Int Coords { get; protected set; }

    public bool IsWorking { get; protected set; }

     public Machine(Vector3Int coords, string id)
     {
        id = Guid.NewGuid().ToString();
        Coords = coords;
     }
    public void Recharge(ResourceSO resource)
    {
        if (!allowedFuels.Contains(resource)) return;
        currentEnergy += resource.energyPerUnit;
        if (currentEnergy > maxEnergy)
            currentEnergy = maxEnergy;
    }
    public IEnumerator Work(RecipeSO recipe)
    {
        if (allowedRecipe.Contains(recipe) && currentEnergy >= recipe.energyCost)
        {
            IsWorking = true;
            currentEnergy -= recipe.energyCost;
            yield return new WaitForSeconds(recipe.completionTime);
            //output goes to a storage to be picked out by a carrier bot
            IsWorking = false;
        }
    }
}
