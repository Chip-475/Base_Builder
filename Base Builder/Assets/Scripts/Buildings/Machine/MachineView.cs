using UnityEngine;

public class MachineView : BuildingView
{
    public new MachineData Data => base.Data as MachineData;
    public new Machine Obj
    {
        get { return base.Obj as Machine; }
        set { base.Obj = value; }
    }
    private void Start()
    {
        Obj = new Machine(Data, this);
    }
}
