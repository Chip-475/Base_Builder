using UnityEngine;

[CreateAssetMenu(fileName = "Bot Data", menuName = "Bots/Bot Data")]
public class BotData : ScriptableObject
{
    public Sprite sprite;
    [Space]
    public BotType type;
    public float speed;
    public int maxWeight;
    public int maxPower;
}
