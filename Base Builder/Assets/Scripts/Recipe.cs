using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public ResourceSO[] inputResources;
    public ResourceSO[] outputResources;

    public string recipeName;
    public string id;

    public int energyCost;

    public float recipeTime;
}
