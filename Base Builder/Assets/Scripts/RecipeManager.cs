using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static Dictionary<string, RecipeSO> RecipeDB { get; private set; } = new();
    public static HashSet<string> UnlockedRecipes { get; private set; } = new();

    public static void Initialize()
    {
        RecipeDB.Clear();
        UnlockedRecipes.Clear();

        RecipeSO[] allRecipes = Resources.LoadAll<RecipeSO>("Recipes");
        foreach (var item in allRecipes)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.ID))
            {
                Debug.LogWarning("Ignoring a recipe without a valid ID.");
                continue;
            }

            if (RecipeDB.ContainsKey(item.ID))
            {
                Debug.LogWarning($"Duplicate recipe ID '{item.ID}' on '{item.name}'.");
                continue;
            }

            RecipeDB[item.ID] = item;

            if (item.unlockedByDefault)
            {
                UnlockedRecipes.Add(item.ID);
                item.isUnlocked = true;
            }
        }
    }
}
