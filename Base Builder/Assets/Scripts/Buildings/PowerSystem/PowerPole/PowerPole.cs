using System;
using UnityEngine;

public class PowerPole : Building
{
    public new PowerPoleData Data => base.Data as PowerPoleData;
    public new PowerPoleView SceneObj => base.SceneObj as PowerPoleView;

    public string poleId;
    public PowerPole(PowerPoleData data, PowerPoleView sceneObj) : base(data, sceneObj)
    {
        poleId = Guid.NewGuid().ToString();
        PowerManager.powerPolesDB[poleId] = this;
        PowerManager.instance.RegisterBuilding();
    }

    public bool CanConnectTo(Building other)
    {
        if (other == null) return false;

        Bounds connectionBounds = new Bounds(SceneObj.transform.position, new Vector3(Data.range * 2, Data.range * 2, 0));
        return connectionBounds.Intersects(other.GetWorldBounds());
    }
    public override void Destroy()
    {
        PowerManager.powerPolesDB.Remove(poleId);
        PowerManager.instance.UnregisterBuilding();
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }
}
