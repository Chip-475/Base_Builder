using UnityEngine;
using UnityEngine.SceneManagement;

public class Machine : Building
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    public bool isPowered { get; private set; }
    public float requestedPower { get; private set; }

    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        PowerManager.machineDB.Add(this);
        PowerManager.instance.RegisterBuilding();
    }
    public override void Destroy()
    {
        FreeUpCells(GetCellsInBounds(GetBounds()));
        MonoBehaviour.Destroy(SceneObj);
    }

    public bool RequestPower(float amount)
    {
        NetworkManager currentNetwork = network;
        if (currentNetwork == null || !currentNetwork.RequestPower(this, amount))
        {
            if (currentNetwork != null) currentNetwork.StopPowerRequest(this);
            requestedPower = 0;
            isPowered = false;
            return false;
        }

        requestedPower = amount;
        isPowered = true;
        return true;
    }
    public bool RequestPower(RecipeSO recipe)
    {
        if (recipe == null) return false;
        return RequestPower(recipe.energyCost);
    }
    public void StopPowerRequest()
    {
        if (network != null) network.StopPowerRequest(this);
        requestedPower = 0;
        isPowered = false;
    }
    public void RefreshPowerRequest()
    {
        RequestPower(requestedPower);
    }
}
