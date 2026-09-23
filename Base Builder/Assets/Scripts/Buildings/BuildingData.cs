using UnityEngine;

public abstract class BuildingData : ScriptableObject
{
    public new string name;
    [TextArea] public string description;
    public Bounds bounds;
    public bool blocksWalking;
    public bool blocksPlacing;
}
