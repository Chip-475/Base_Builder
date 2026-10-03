using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class Machine : Building
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;
<<<<<<< Updated upstream
=======
        public MachineType Type { get; protected set; } = MachineType.None;

>>>>>>> Stashed changes
    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        // fill out as necessary
        PowerManager.instance.machineDB.Add(this);
        Tester.Instance.machines.Add(this); // testing
    }
    public async UniTask UseSmelter()
    {
        if(Type == MachineType.Smelter)
        {

           
        }
    }
    public async UniTask UseAssembler()
    {
        if(Type == MachineType.Assembler)
        {
            
        }
    }
    public async UniTask UseAdvancedAssembler()
    {
        if(Type == MachineType.Advanced_Assembler)
        {
            
        }
    }
    public async UniTask UseAlloyFurnace()
    {
        if(Type == MachineType.Alloy_Furnace)
        {
            
        }
    }
    public async UniTask UsePress()
    {
        if(Type == MachineType.Press)
        {
            
        }
    }
    public async UniTask UseMachine()
    {
        switch(Type)
        {
            case MachineType.Smelter:
                await UseSmelter();
                break;
            case MachineType.Assembler:
                await UseAssembler();
                break;
            case MachineType.Advanced_Assembler:
                await UseAdvancedAssembler();
                break;
            case MachineType.Alloy_Furnace:
                await UseAlloyFurnace();
                break;
            case MachineType.Press:
                await UsePress();
                break;
        }
    }

    public enum MachineType
{
    None,
    Advanced_Assembler,
    Alloy_Furnace,
    Assembler,
    Press,
    Smelter,
}
}
