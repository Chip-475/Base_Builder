using UnityEngine;

public class MineralCrateView : BuildingView
{
    public new MineralCrateData Data => base.Data as MineralCrateData;
    public new MineralCrate RuntimeObj
    {
        get { return base.RuntimeObj as MineralCrate; }
        set { base.RuntimeObj = value; }
    }

    private void Start()
    {
        RuntimeObj = new MineralCrate(Data, this);
    }
    private void Update()
    {
        if (!RuntimeObj.Inventory.IsEmpty)
            RuntimeObj.IssueLogisticsRequest();
    }
}
