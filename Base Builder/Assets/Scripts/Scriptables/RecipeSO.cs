using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string ID { get; protected set; }
    public string Name;
    [TextArea] public string Desc;
    public Sprite Sprite;

    [Header("Characteristics")]
    [SerializeField] public Dictionary<ResourceSO, int> inputs = new();
    [SerializeField] public Dictionary<ResourceSO, int> outputs = new();
    public float completionTime;
    public int energyCost;
}
    public enum RecipeType
{
    None=0,
    Smelter=1,
    Assembler=2,
    Advanced_Assembler=3,
    Alloy_Furnace=4,
    Press=5
}
