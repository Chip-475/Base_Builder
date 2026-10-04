using UnityEngine;

public class MachineView : BuildingView
{
    public new MachineData Data => base.Data as MachineData;
    public new Machine Obj
    {
        get { return base.Obj as Machine; }
        set { base.Obj = value; }
    }
    private Machine machine;

    private void Start()
    {
        machine = new Machine(Data, this);
    }
    private void OnDestroy()
    {
        if (machine == null) return;
        machine.StopPowerRequest();
        PowerManager.machineDB.Remove(machine);
        if (PowerManager.instance != null) PowerManager.instance.UnregisterBuilding();
    }
}
