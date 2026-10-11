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
     public async UniTask<ResourceSO[]> UseMachine(RecipeSO recipe, ResourceSO[] inputResources,MachineType type,RecipeType recipeType)
    {
            if (recipe != null && RequestPower(recipe) && (int)recipeType==(int)type)
            {
                await UniTask.Delay((int)(recipe.completionTime * 1000)); // da vedere quanto fare che duri ogni processo
                ResourceSO[] WorkedResources = recipe.outputResources;
                return WorkedResources;
            }
        return null;
    }
}
public enum MachineType
{
    None=0,
    Smelter=1,
    Assembler=2,
    Advanced_Assembler=3,
    Alloy_Furnace=4,
    Press=5
}
