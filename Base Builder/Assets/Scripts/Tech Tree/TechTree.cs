using System.Collections.Generic;
using UnityEngine;

public class TechTree
{
    public Dictionary<string, Tech> TechDB { get; private set; } = new();

    public void Initialize()
    {
        Tech[] techAssets = Resources.LoadAll<Tech>("Tech");
        foreach (Tech tech in techAssets)
            TechDB[tech.id] = tech;
    }
    public void Load(Tech.SaveData[] saveData)
    {
        foreach(Tech.SaveData data in saveData)
            TechDB[data.id].unlocked = data.unlocked;
    }

    public bool IsTechUnlocked(string id)
    {
        if (!TechDB.ContainsKey(id))
            return false;

        return TechDB[id].unlocked;
    }
}
