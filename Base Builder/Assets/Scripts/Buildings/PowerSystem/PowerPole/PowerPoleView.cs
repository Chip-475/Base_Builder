using UnityEngine;

public class PowerPoleView : BuildingView
{
    public new PowerPoleData Data => base.Data as PowerPoleData;
    public override void Init()
    {

    }
    private void Start()
    {
        new PowerPole(Data, this);
    }
}
