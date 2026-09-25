using UnityEngine;
using UnityEngine.Rendering;

public class Machine : Building
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        // fill out as necessary
        Tester.Instance.machines.Add(this); // testing
    }
}
