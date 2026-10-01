using UnityEngine;

public class MachineView : BuildingView
{
    public new MachineData Data => base.Data as MachineData;
    //public Machine machine => Building as Machine;
    private void Start()
    {
        new Machine(Data, this);
    }
}
