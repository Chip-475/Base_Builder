using UnityEngine;

public class WorkerBotView : BotView
{
    public new WorkerBot RuntimeObj
    {
        get { return base.RuntimeObj as WorkerBot; }
        set { base.RuntimeObj = value; }
    }

    protected override void Start()
    {
        RuntimeObj = new WorkerBot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
}
