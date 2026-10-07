using UnityEngine;
<<<<<<< Updated upstream
=======
using UnityEngine.SceneManagement;
>>>>>>> Stashed changes
using Cysharp.Threading.Tasks;

public class Machine : Building
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    public bool Powered { get; private set; }
    public float RequestedPower { get; private set; }
    public MachineType Type { get; protected set; } = MachineType.None;

    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        PowerManager.machineDB.Add(this);
        PowerManager.instance.RegisterBuilding();
    }
    public override void Destroy()
    {
        FreeUpCells(GetCellsInBounds(GetBounds()));
        //WorldManager.World.UnregisterBuilding(Coords);
        MonoBehaviour.Destroy(SceneObj);
    }

    public bool RequestPower(float amount)
    {
        NetworkManager currentNetwork = Network;
        if (currentNetwork == null || !currentNetwork.RequestPower(this, amount))
        {
            if (currentNetwork != null) currentNetwork.StopPowerRequest(this);
            RequestedPower = 0;
            Powered = false;
            return false;
        }

        RequestedPower = amount;
        Powered = true;
        return true;
    }
    public bool RequestPower(RecipeSO recipe)
    {
        if (recipe == null) return false;
        return RequestPower(recipe.energyCost);
    }
    public void StopPowerRequest()
    {
        if (Network != null) Network.StopPowerRequest(this);
        RequestedPower = 0;
        Powered = false;
    }
    public void RefreshPowerRequest()
    {
        RequestPower(RequestedPower);
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
public MachineType Type { get; protected set; } = MachineType.None;
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
public enum MachineType
{
    None,
    Advanced_Assembler,
    Alloy_Furnace,
    Assembler,
    Press,
    Smelter,
}
