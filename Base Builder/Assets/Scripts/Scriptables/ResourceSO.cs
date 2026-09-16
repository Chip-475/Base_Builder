using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resource")]
public class ResourceSO : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string ID { get; protected set;}
    public string r_name;
    [TextArea] public string r_desc;
    public Sprite r_sprite;

    [Header("Characteristics")]
    public int toughness = 1;
    public float weightPerUnit = 1f;
    public int energyPerUnit = 0;
}
