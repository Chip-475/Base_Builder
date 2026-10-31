using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "Tech", menuName = "Tech")]
public class TechSO : ScriptableObject, ISerializationCallbackReceiver
{
    [Serializable]
    public struct costEntry
    {
        public ResourceSO resource;
        public int quant;

    }
    [Tooltip("Set by hand.")] public string id;
    [Tooltip("attiva se sbloccata subito")] public bool unlock;
    [Space]
    public new string name;
    public Sprite icon;
    public List<RecipeSO> recipeUnlocks;
    public List<ResourceSO> ResourcesUnlocks;
    [SerializeField]
    public List<MachineData> MachineUnlocks;

    [SerializeField] public Dictionary<ResourceSO, int> cost = new();
    [SerializeField] public List<costEntry> costList=new List<costEntry>(); // testare prima con il dizionario

    private void rebuildCost()
    {
        cost=new Dictionary<ResourceSO, int>();
        foreach(costEntry voce in costList)
        {
            if(voce.resource!=null)cost[voce.resource]=voce.quant;
        }
    }

    public void OnBeforeSerialize()
    {
        
    }
    public void OnAfterDeserialize()
    {
        rebuildCost();
    }

    public void OnValidate()
    {
        rebuildCost();
    }

}
