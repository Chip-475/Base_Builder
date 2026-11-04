using UnityEngine;

public class BuilderBotView : BotView
{
    protected override void Start()
    {
        RuntimeObj = new BuilderBot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
}
