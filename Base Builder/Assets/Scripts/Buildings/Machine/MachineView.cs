using UnityEngine;

public class MachineView : BuildingView
{
    public MachineData Data => data as MachineData;

    private void Start()
    {
        new Machine(Data, this);
    }
}
