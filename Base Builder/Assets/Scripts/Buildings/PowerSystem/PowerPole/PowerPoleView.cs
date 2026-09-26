using UnityEngine;

public class PowerPoleView : BuildingView
{
    public new PowerPoleData Data => base.Data as PowerPoleData;
    public PowerPole PowerPole => Building as PowerPole;
    void Awake()
    {
        Building=new PowerPole(Data, this);
    }
}
