using UnityEngine;

[CreateAssetMenu(fileName = "Generic Building Data", menuName = "Buildings/Generic Building")]
public abstract class BuildingData : ScriptableObject
{
    public new string name;
    [TextArea] public string description;
    public Sprite sprite;
    public BuildingType type;
    public Bounds bounds;
    public Bounds interactionBounds;
    public Bounds connectionBounds;
    public bool connectsToPower;
    public bool blocksWalking;
    public bool blocksPlacing;
}
public enum BuildingType
{
    None,
    Machine,
    Mining,
    Power,
    Storage
}
