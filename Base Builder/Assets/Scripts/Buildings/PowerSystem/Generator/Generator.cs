using System;
using System.Collections.Generic;
using UnityEngine;
public class Generator : Building
{
    public new GeneratorData Data => base.Data as GeneratorData;
    public new GeneratorView SceneObj => base.SceneObj as GeneratorView;

    public string id;

    public float Power { get { return Data.productionRate; } }
    public bool running { get; private set; }

    public bool isPowered;

    public Dictionary<ResourceSO, float> inventory;
    public Generator(GeneratorData data, GeneratorView sceneObj) : base(data, sceneObj)
    {
        id = Guid.NewGuid().ToString();
        PowerManager.powerGeneratorDB[id] = this;
        PowerManager.instance.RegisterBuilding();
    }
    public void SwitchState(bool state) //to link to the ui button
    {
        running = state;
        if (network != null) network.RefreshPower();
    }
    public override void Destroy()
    {
        PowerManager.powerGeneratorDB.Remove(id);
        PowerManager.instance.UnregisterBuilding();
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }
}
