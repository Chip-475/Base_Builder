using UnityEngine;

public class MachineView : BuildingView
{
    public new MachineData Data => base.Data as MachineData;

    private void Start()
    {
        new Machine(Data, this);
    }
}
