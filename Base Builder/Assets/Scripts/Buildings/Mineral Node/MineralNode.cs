using UnityEngine;

public class MineralNode : Building
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public new MineralNodeView SceneObj => base.SceneObj as MineralNodeView;

    public MineralNode(MineralNodeData data, MineralNodeView sceneObj) : base(data, sceneObj)
    {
        
    }
    public override void Destroy()
    {
        BuildingManager.RegisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj);
    }

    public (ResourceSO resource, int quantity) MineResource()
    {
        //bot.Mine qua ci va la funzione del bot che aspetta oer minare il materiale
        ResourceSO type = Data.resource;
        int mineralQuantity = Data.baseAmountGiven * (int)Data.purity;
        return (type, mineralQuantity);
    }
}
