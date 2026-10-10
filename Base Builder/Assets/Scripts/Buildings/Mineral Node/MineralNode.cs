using UnityEngine;

public class MineralNode : Building
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public new MineralNodeView SceneObj => base.SceneObj as MineralNodeView;

    public MineralNode(MineralNodeData data, MineralNodeView sceneObj) : base(data, sceneObj)
    {
        // una funzione scava che deve tornare che risorsa ti sta dando e quanta
        //ogni node ha quanto ti da di base di una risorsa e una purezza amount*purezza(int)
        //avere una funzione che torna risorsa data e quantità data
    }
    public override void Destroy()
    {
        WorldManager.World.UnregisterBuilding(Coords);
        MonoBehaviour.Destroy(SceneObj);
    }

    public (ResourceSO resource, int quantity) MineResource()
    {
        //bot.Mine qua ci va la funzione del bot che aspetta per minare il materiale
        ResourceSO type = Data.resource;
        int mineralQuantity = Data.baseAmountGiven * (int)Data.purity;
        return (type, mineralQuantity);
    }
}
