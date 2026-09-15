using UnityEngine;

[CreateAssetMenu(fileName = "Resources", menuName = "Scriptable Objects/Resources")]

public class Resources : ScriptableObject
{
    [field: SerializeField] public string ResourceID { get; private set;}
    public string resourceName;
    [TextArea]public string resourceDescription;
    public Sprite resourceSprite;
    public int toughtness=1;
    public int weightPerUnit;
    public int energyPerUnit;
}
