using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Generic Building Data", menuName = "Buildings/Generic Building")]
public abstract class BuildingData : ScriptableObject
{
    public string ID;
    [Space]
    public new string name;
    [TextArea] public string description;
    public Sprite sprite;
    [SerializeField] public Dictionary<ResourceSO, int> cost = new();
    public BuildingType type;
    public Bounds bounds;
    public Bounds interactionBounds;
    public Bounds connectionBounds;
    public float workRange;
    public bool connectsToPower;
    public bool blocksWalking;
    public bool blocksPlacing;
    [Min(0.1f)] public float constructionTime = 1f;
}
public enum BuildingType
{
    None,
    Machine,
    Mining,
    Power,
    Storage
}
