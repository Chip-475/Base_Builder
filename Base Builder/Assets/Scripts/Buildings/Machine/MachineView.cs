using UnityEngine;

public class MachineView : BuildingView
{
    public new MachineData Data => base.Data as MachineData;
    public Machine Machine => Building as Machine;
    void Awake()
    {
        Building=new Machine(Data, this);
    }
    
    void OnMouseDown()
    {
        if(machineRecipeUI.instance != null && Machine != null) machineRecipeUI.instance.apri(Machine);
    }
}
