using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Tech", menuName = "Tech")]
public class Tech : ScriptableObject
{
    public class SaveData
    {
        public string id;
        public bool unlocked;
    }
    [Serializable]
    public struct costo
    {
        public ResourceSO res;
        public int quant;
    }
    [Tooltip("Set by hand.")] public string id;
    public bool unlocked;
    [Space]
    public new string name;
    public Sprite icona;
    [SerializeField]
    public List<costo> costoList=new List<costo>();
    [SerializeField] public Dictionary<ResourceSO, int> cost = new();

    public void OnAfterDeserialize()
    {
        cost=new Dictionary<ResourceSO, int>();
        foreach(costo voce in costoList)
        {
            cost[voce.res]=voce.quant;
        }
    }

    public void Unlock()
    {
        if (!IsUnlockable())
            return;

        foreach(KeyValuePair<ResourceSO,int> voce in cost)
        {
            Depot.Inventory.Items[voce.Key]-=voce.Value;
        }

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
