using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

[CreateAssetMenu(fileName = "Tech", menuName = "Tech")]
public class TechSO : ScriptableObject
{
    [Tooltip("Set by hand.")] public string id;
    [Space]
    public new string name;
    public Sprite icon;
    [Space]
    public List<BotManager.BotUpgrade> botUpgrades = new();
    public List<RecipeSO> recipeUnlocks = new();
    public List<BuildingData> buildingUnlocks = new();

    [SerializeField] public Dictionary<ResourceSO, int> cost = new();

    public void ApplyEffects()
    {
        // Bot Upgrades
        foreach (BotManager.BotUpgrade upgrade in botUpgrades)
        {
            upgrade.data.speed += upgrade.deltaSpeed;
            upgrade.data.maxWeight += upgrade.deltaMaxWeight;
            upgrade.data.miningLevel += upgrade.deltaMiningLevel;
            upgrade.data.powerConsumption += upgrade.deltaPowerConsumption;
            upgrade.data.maxPower -= upgrade.deltaMaxPower;
        }

        // Recipes

        // Buildings
        foreach (BuildingData data in buildingUnlocks)
            BuildingManager.UnlockBuilding(data.ID);
    }
}
