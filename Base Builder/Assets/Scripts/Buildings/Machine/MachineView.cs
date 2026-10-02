using UnityEngine;

public class MachineView : BuildingView
{
    public new MachineData Data => base.Data as MachineData;
    public Machine machine =>  base.bulding as Machine;
    public override void Init()
    {
        bulding = new Machine(Data, this);
    }
    public void OnMouseDown()
    {
        if (machineRecipeUI.instance != null && machine != null) machineRecipeUI.instance.apri(machine);
    }
}
