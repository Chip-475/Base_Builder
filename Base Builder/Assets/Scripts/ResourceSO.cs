using UnityEngine;

[CreateAssetMenu(fileName = "Resources", menuName = "Scriptable Objects/Resources")]
public class ResourceSO : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string ResourceID { get; private set;}
    public string r_name;
    [TextArea] public string r_desc;
    public Sprite r_sprite;

    [Header("Stats")]
    public int toughness = 1;
    public int weightPerUnit;
    public int energyPerUnit;
}
