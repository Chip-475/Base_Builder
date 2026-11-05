using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static Dictionary<string, RecipeSO> RecipeDB { get; private set; } = new();
    public static HashSet<string> UnlockedRecipes { get; private set; } = new();

    public static void Initialize()
    {
        RecipeSO[] allRecipes = Resources.LoadAll<RecipeSO>("Recipes");
        foreach (var item in allRecipes)
        {
            RecipeDB[item.ID] = item;

            if (item.unlockedByDefault)
                UnlockedRecipes.Add(item.ID);
        }
    }
}
