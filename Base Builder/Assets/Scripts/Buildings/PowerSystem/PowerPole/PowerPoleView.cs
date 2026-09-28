using UnityEngine;

public class PowerPoleView : BuildingView
{
    public new PowerPoleData Data => base.Data as PowerPoleData;

    private void Start()
    {
        new PowerPole(Data, this);
    }
}
