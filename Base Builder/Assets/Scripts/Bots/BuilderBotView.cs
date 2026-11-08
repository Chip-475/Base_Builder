using UnityEngine;

public class BuilderBotView : BotView
{
    protected override void Start()
    {
        InitializeRuntime();
    }

    public override void InitializeRuntime()
    {
        if (RuntimeObj == null)
            RuntimeObj = new BuilderBot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
}
