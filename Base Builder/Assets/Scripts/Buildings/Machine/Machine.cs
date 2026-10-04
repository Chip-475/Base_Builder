using UnityEngine;

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
    public bool isPowered { get; private set; }
    public float requestedPower { get; private set; }
    public NetworkManager network
    {
        get
        {
            PowerManager.instance.checkNetwork(this, out NetworkManager result);
            return result;
        }
    }
    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        PowerManager.machineDB.Add(this);
        if (Tester.Instance != null) Tester.Instance.machines.Add(this);
        PowerManager.instance.RegisterBuilding();
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
    public override void Destroy()
    {
        StopPowerRequest();
        PowerManager.machineDB.Remove(this);
        if (Tester.Instance != null) Tester.Instance.machines.Remove(this);
        PowerManager.instance.UnregisterBuilding();
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }
}
