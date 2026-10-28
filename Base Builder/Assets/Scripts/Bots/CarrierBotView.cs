using UnityEngine;

public class CarrierBotView : BotView
{
    public new CarrierBot RuntimeObj
    {
        get { return base.RuntimeObj as CarrierBot; }
        set { base.RuntimeObj = value; }
    }

    protected override void Start()
    {
        RuntimeObj = new CarrierBot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
}
