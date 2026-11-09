using System.Collections.Generic;
using UnityEngine;

public class TechTree
{
    public static Dictionary<string, TechSO> TechDB { get; private set; } = new();
    public static HashSet<string> UnlockedTechs { get; private set; } = new();

    public static void Initialize()
    {
        TechDB.Clear();
        TechSO[] techAssets = Resources.LoadAll<TechSO>("Tech");
        foreach (TechSO tech in techAssets)
            TechDB[tech.ID] = tech;
    }

    public static void UnlockTech(TechSO tech)
    {
        if (!IsTechUnlockable(tech)) return;

        if (!Depot.Inventory.CanRemoveItems(tech.cost)) return;
            Depot.Inventory.RemoveItems(tech.cost);

        foreach (RecipeSO re in tech.recipeUnlocks)
            re.isUnlocked = true;
        
        UnlockedTechs.Add(tech.ID);
        tech.ApplyEffects();
        AudioManager.PlaySFX(AudioManager.Instance.clips["New Node"], PlayerManager.Instance.transform);
    }
    public static bool IsTechUnlockable(TechSO tech)
    {
        if (UnlockedTechs.Contains(tech.ID))
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
    public bool IsTechUnlocked(string id) => UnlockedTechs.Contains(id);
}
