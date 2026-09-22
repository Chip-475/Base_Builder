using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string ID { get; protected set; }
    public string r_name;

    [Header("Characteristics")]
    public ResourceSO[] inputResources;
    public int[] input;
    public ResourceSO[] outputResources;
    public int[] output;
    public float completionTime;
    public int energyCost;
}
