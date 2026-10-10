using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string ID { get; protected set; }
    public string Name;

    [Header("Characteristics")]
    [SerializeField] public Dictionary<ResourceSO, int> inputs = new();
    [SerializeField] public Dictionary<ResourceSO, int> outputs = new();
    public float completionTime;
    public int energyCost;
}
