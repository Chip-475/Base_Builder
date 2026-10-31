using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "Tech", menuName = "Tech")]
public class TechSO : ScriptableObject
{
    [Tooltip("Set by hand.")] public string id;
    [Space]
    public new string name;
    public Sprite icon;
    public List<RecipeSO> recipeUnlocks;
    [SerializeField] public Dictionary<ResourceSO, int> cost = new();
}
