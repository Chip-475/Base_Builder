using UnityEngine;
using UnityEngine.Rendering;

public class Machine : Building
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        
    }
    public override void Destroy()
    {
        FreeUpCells(GetCellsInBounds(Data.bounds));
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }
}
