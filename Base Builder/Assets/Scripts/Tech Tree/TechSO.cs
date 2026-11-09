using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tech", menuName = "Tech")]
public class TechSO : ScriptableObject
{
    [Tooltip("Set by hand.")] public string ID;
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
            if (upgrade == null || upgrade.data == null)
                continue;

            upgrade.data.speed += upgrade.deltaSpeed;
            upgrade.data.maxWeight = Mathf.Max(0, upgrade.data.maxWeight + upgrade.deltaMaxWeight);
            upgrade.data.miningLevel += upgrade.deltaMiningLevel;
            upgrade.data.powerConsumption = Mathf.Max(0, upgrade.data.powerConsumption + upgrade.deltaPowerConsumption);
            upgrade.data.maxPower = Mathf.Max(1, upgrade.data.maxPower + upgrade.deltaMaxPower);
            BotManager.ApplyUpgrade(upgrade.data);
        }

        // Recipes
        foreach (RecipeSO recipe in recipeUnlocks)
            if (recipe != null && !string.IsNullOrWhiteSpace(recipe.ID))
                RecipeManager.UnlockedRecipes.Add(recipe.ID);

        // Buildings
        foreach (BuildingData data in buildingUnlocks)
            if (data != null)
                BuildingManager.UnlockBuilding(data.ID);
    }
}
