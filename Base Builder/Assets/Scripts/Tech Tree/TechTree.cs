using System.Collections.Generic;
using UnityEngine;

public class TechTree
{
    public static Dictionary<string, TechSO> TechDB { get; private set; } = new();
    public static HashSet<string> UnlockedTechs { get; private set; } = new();

    public void Initialize()
    {
        TechDB.Clear();
        TechSO[] techAssets = Resources.LoadAll<TechSO>("Tech");
        foreach (TechSO tech in techAssets)
        {
            if(string.IsNullOrEmpty(tech.id))
            {
                Debug.Log("tech senza id dio..");
                return;
            }
            if(TechDB.ContainsKey(tech.id))
            {
                Debug.Log("duplicato dio cane");
                return;
            }
            TechDB[tech.id] = tech;
            foreach(RecipeSO ricetta in tech.recipeUnlocks)
            {
                if(ricetta!=null)ricetta.isUnlocked=false;
            }
        }
        foreach(TechSO tech in TechDB.Values)
        {
            if(tech.unlock)applSblocco(tech);
        }   
    }

    private static void applSblocco(TechSO tech)
    {
        foreach(RecipeSO ric in tech.recipeUnlocks)
        {
            if(ric!=null)ric.isUnlocked=true;
        }
    }  

    public static void UnlockTech(TechSO tech)
    {
        if (!IsTechUnlockable(tech)) return;

        if (!Depot.Inventory.CanRemoveItems(tech.cost)) return;
        Depot.Inventory.RemoveItems(tech.cost);

        //foreach (RecipeSO re in tech.recipeUnlocks) re.isUnlocked = true;
        //UnlockedTechs.Add(tech.id);
        applSblocco(tech);
    }
    public static bool isLocked(TechSO tech)
    {
        return tech.unlock||UnlockedTechs.Contains(tech.id);
    }

    public static bool canAfford(TechSO tech)
    {
        if(Depot.Inventory==null)return false;
        foreach(KeyValuePair<ResourceSO,int>voce in tech.cost)
        {
            Depot.Inventory.Items.TryGetValue(voce.Key,out int posseduta);
            if(posseduta<voce.Value)return false;
        }
        return true;
    }

    public static bool IsTechUnlockable(TechSO tech)
    {
        if (UnlockedTechs.Contains(tech.id))
            return false;

        foreach (var kvp in tech.cost)
        {
            if (!Depot.Inventory.Items.ContainsKey(kvp.Key))
                return false;
            if (Depot.Inventory.Items[kvp.Key] < kvp.Value)
                return false;
        }

        return true;
    }

    public static bool techVisible(TechSO tech)
    {
        return isLocked(tech)||canAfford(tech);
    }

    public bool IsTechUnlocked(string id) => UnlockedTechs.Contains(id);
}
