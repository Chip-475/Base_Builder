using UnityEngine;
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
     public async UniTask<ResourceSO[]> UseSmelter(RecipeSO recipe, ResourceSO[] inputResources)
    {
        if (Type == MachineType.Smelter)
        {
            if (recipe != null && RequestPower(recipe))
            {
                await UniTask.Delay((int)(recipe.completionTime * 1000)); // da vedere quanto fare che duri ogni processo
                ResourceSO[] WorkedResources = recipe.outputResources;
                return WorkedResources;
            }
        }
        return null;
    }
    public async UniTask<ResourceSO[]> UseAssembler(RecipeSO recipe, ResourceSO[] inputResources)
    {
        if (Type == MachineType.Assembler)
        {
            if (recipe != null && RequestPower(recipe))
            {
                await UniTask.Delay((int)(recipe.completionTime * 1000)); // da vedere quanto fare che duri ogni processo
                ResourceSO[] WorkedResources = recipe.outputResources;
                return WorkedResources;
            }
        }
        return null;
    }
    public async UniTask<ResourceSO[]> UseAdvancedAssembler(RecipeSO recipe, ResourceSO[] inputResources)
    {
        if (Type == MachineType.Advanced_Assembler)
        {
            if (recipe != null && RequestPower(recipe))
            {
                await UniTask.Delay((int)(recipe.completionTime * 1000)); // da vedere quanto fare che duri ogni processo
                ResourceSO[] WorkedResources = recipe.outputResources;
                return WorkedResources;
            }
        }
        return null;
    }
    public async UniTask<ResourceSO[]> UseAlloyFurnace(RecipeSO recipe, ResourceSO[] inputResources)
    {
        if (Type == MachineType.Alloy_Furnace)
        {
            if (recipe != null && RequestPower(recipe))
            {
                await UniTask.Delay((int)(recipe.completionTime * 1000)); // da vedere quanto fare che duri ogni processo
                ResourceSO[] WorkedResources = recipe.outputResources;
                return WorkedResources;
            }
        }
        return null;
    }
    public async UniTask<ResourceSO[]> UsePress(RecipeSO recipe, ResourceSO[] inputResources)
    {
        if (Type == MachineType.Press)
        {
            if (recipe != null && RequestPower(recipe))
            {
                await UniTask.Delay((int)(recipe.completionTime * 1000)); // da vedere quanto fare che duri ogni processo
                ResourceSO[] WorkedResources = recipe.outputResources;
                return WorkedResources;
            }
        }
        return null;
    }
    public async UniTask UseMachine(RecipeSO recipe, ResourceSO[] inputResources)
    {
        switch (Type)
        {
            case MachineType.Smelter:
                await UseSmelter(recipe, inputResources);
                break;
            case MachineType.Assembler:
                await UseAssembler(recipe, inputResources);
                break;
            case MachineType.Advanced_Assembler:
                await UseAdvancedAssembler(recipe, inputResources);
                break;
            case MachineType.Alloy_Furnace:
                await UseAlloyFurnace(recipe, inputResources);
                break;
            case MachineType.Press:
                await UsePress(recipe, inputResources);
                break;
        }
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
