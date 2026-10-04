using UnityEngine;

public class PowerPoleView : BuildingView
{
    public new PowerPoleData Data => base.Data as PowerPoleData;
    private PowerPole powerPole;

    private void Start()
    {
        powerPole = new PowerPole(Data, this);
    }
    private void OnDestroy()
    {
        if (powerPole == null) return;
        PowerManager.powerPolesDB.Remove(powerPole.poleId);
        if (PowerManager.instance != null) PowerManager.instance.UnregisterBuilding();
    }
}
