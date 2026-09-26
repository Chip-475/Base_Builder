using UnityEngine;

public class Depot : Building
{
    public new DepotData Data => base.Data as DepotData;
    public new DepotView SceneObj => base.SceneObj as DepotView;

    private static DepotInventory sharedInventory = new();

    public Depot(DepotData data, DepotView sceneObj) : base(data, sceneObj)
    {
        //deposito univoco dove possiamo mettere tutto quanto
        //input output ogni depot in cui metti qualcosa sono collegati fra loro e quando prendi qualcosa da uno si toglie da tutti 
        //funzioni per prendere e posare roba 
        //dizionario con chiave la risorsa e valore quantita
        
    }

    public (ResourceSO resource, int quantity) AddItem(ResourceSO resource, int quantity)
    {
        return sharedInventory.AddItem(resource, quantity);
        
    }

    public (ResourceSO resource, int quantity) RemoveItem(ResourceSO resource, int quantity)
    {
        return sharedInventory.RemoveItem(resource, quantity);
    }

    public int GetQuantity(ResourceSO resource)
    {
        return sharedInventory.GetQuantity(resource);
    }
}