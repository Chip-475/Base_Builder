using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "Tech", menuName = "Tech")]
public class Tech : ScriptableObject
{
    public class SaveData
    {
        public string id;
        public bool unlocked;
    }
    [Tooltip("Set by hand.")] public string id;
    public bool unlocked;
    [Space]
    public new string name;
    public Sprite icon;
    public List<RecipeSO> recipes;
    [SerializeField] public Dictionary<ResourceSO, int> cost = new();


    public void Unlock()
    {
        if (!IsUnlockable()) return;
        if (!Depot.Inventory.CanRemoveItems(cost)) return;
        Depot.Inventory.RemoveItems(cost);
        foreach (RecipeSO re in recipes) re.isUnlocked = true;
        unlocked = true;
    }
    public bool IsUnlockable()
    {
        if (unlocked)
            return false;
        foreach (var kvp in cost)
        {
            if (!Depot.Inventory.Items.ContainsKey(kvp.Key))
                return false;
            if (Depot.Inventory.Items[kvp.Key] < kvp.Value)
                return false;
        }

        return true;
    }
    
    public SaveData GetSaveData()
    {
        return new()
        {
            id = id,
            unlocked = unlocked,
        };
    }
}
