using UnityEngine;

[CreateAssetMenu(fileName = "BotData", menuName = "Scriptable Objects/BotData")]
public class BotData : ScriptableObject
{
    public int speed;
    public Sprite sprite;
    public BotType type;
    public int carryCapacity;


}
