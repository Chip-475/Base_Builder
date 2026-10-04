using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resource")]
public class ResourceSO : ScriptableObject
{
    [Header("Identity")]
    public string ID { get; private set;}
    [Space]
    public string Name;
    [TextArea] public string Desc;
    public Sprite Sprite;

    [Header("Characteristics")]
    public int toughness = 1;
    public float weightPerUnit = 1f;
    public int energyPerUnit = 0;

    #if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    #endif
}
