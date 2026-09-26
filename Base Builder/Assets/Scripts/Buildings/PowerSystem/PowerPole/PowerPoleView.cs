using UnityEngine;

public class PowerPoleView : BuildingView
{
    public new PowerPoleData Data => base.Data as PowerPoleData;
    
    private void Start()
    {
        var pp = new PowerPole(Data, this);
        pp.OnDestroy+=(pp)=>
    }
}
