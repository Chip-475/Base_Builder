using UnityEngine;
using System.Collections;

public class Machine : InstalledObject
{
    //lista di ricette che la macchina può eseguire,
    // prendono un imput e tornano un output facendo lavorare il bot per x tempo 

    public int energyLevel;

    //coal value=50 
     public ResourceSO[] allowedResource= { };

    [Header("Identity")]
    [field: SerializeField] public string MachineID { get; private set;}
    public string m_name;
    [TextArea] public string m_desc;
    public Sprite m_sprite;

    public Vector3Int Coords { get; protected set; }

    public bool IsWorking { get; protected set; }

    public IEnumerator Work(RecipeSO recipe)
    {
        IsWorking = true;
        yield return new WaitForSeconds(recipe.completionTime);
        IsWorking = false;
    }

    public void inizializza(Vector3Int cords, string idMachina)
    {
        Coords = cords;
        MachineID = idMachina;
    }

    public bool rifEnergia(Bot bot,ResourceSO risorsaCombustibile,int quantita)
    {
        bool rim=bot.rimuoviRes(risorsaCombustibile,quantita);
        if (!rim) return false;
        energyLevel=energyLevel+(risorsaCombustibile.energyPerUnit*quantita);
        return true;
    }
    
    /*
    public int EnergyLevel(ResourceSO resource)
    {
        // resource -= 1; devo sottrarre una risorsa dal magazzino/inventario
        energyLevel += resource.energyPerUnit;
        return energyLevel;
    }

     public Machine(Vector3Int coords, string id)
     {
        string[] recipes = { "Recipe1", "Recipe2", "Recipe3" };
        Coords = coords;

     }*/
}
