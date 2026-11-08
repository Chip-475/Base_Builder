using UnityEngine;

public class MinerBotView : BotView
{
    public new MinerBot RuntimeObj
    {
        get { return base.RuntimeObj as MinerBot; }
        set { base.RuntimeObj = value; }
    }

    protected override void Start()
    {
        InitializeRuntime();
    }

    public override void InitializeRuntime()
    {
        if (RuntimeObj == null)
            RuntimeObj = new MinerBot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
}
