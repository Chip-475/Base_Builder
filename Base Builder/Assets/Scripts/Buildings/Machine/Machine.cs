using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;
using Cysharp.Threading.Tasks;
public class Machine : Building, IInteractable
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public bool Powered { get; private set; }
    public float RequestedPower { get; private set; }
    public MachineType Type { get; protected set; } = MachineType.None;

    public Dictionary<ResourceSO, int> input{get; private set;}=new Dictionary<ResourceSO, int>();
    public Dictionary<ResourceSO,int> output{get; private set;}=new Dictionary<ResourceSO, int>();
    public bool lavoro{get;private set;}
    public bool aspeEne{get;private set;}
    public RecipeSO ricOra{get; private set;}
    public float progresso{get; private set;}

    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        PowerManager.machineDB.Add(this);
        PowerManager.instance.RegisterBuilding();
        InteractionManager.Instance.interactables.Add(this);
    }
    public override void Destroy()
    {
        FreeUpCells(GetCellsInBounds(GetBounds()));
        BuildingManager.UnregisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public bool RequestPower(float amount)
    {
        NetworkManager currentNetwork = Network;
        if (currentNetwork == null || !currentNetwork.RequestPower(this, amount))
        {
            if (currentNetwork != null) currentNetwork.StopPowerRequest(this);
            RequestedPower = 0;
            Powered = false;
            return false;
        }

        RequestedPower = amount;
        Powered = true;
        return true;
    }
    public bool RequestPower(RecipeSO recipe)
    {
        if (recipe == null) return false;
        return RequestPower(recipe.energyCost);
    }
    public void StopPowerRequest()
    {
        if (Network != null) Network.StopPowerRequest(this);
        RequestedPower = 0;
        Powered = false;
    }
    public void RefreshPowerRequest()
    {
        RequestPower(RequestedPower);
    }

    public bool start(RecipeSO ric)
    {
        if(ric==null||lavoro||Data.usableRecipes==null||!Data.usableRecipes.Contains(ric))return false;
        foreach(KeyValuePair<ResourceSO,int> richiesta in ric.inputs)
        {
            Depot.Inventory.Items.TryGetValue(richiesta.Key,out int posseduta);
            if(posseduta<richiesta.Value)return false;
        }
        return true;
    }

    public void avvioAuto()
    {
        if(lavoro||Data.usableRecipes==null)return;
        foreach(RecipeSO recipe in Data.usableRecipes)
        {
            if(!start(recipe))continue;
            if(recipe.energyCost>0)//)
            {
                aspeEne=true;
                return;
            }
        }
        aspeEne=false;
        //funzione craft
        return;
    }

    async UniTaskVoid prova()
    {
        var token=SceneObj.GetCancellationTokenOnDestroy();
        while(true)
        {
            bool annulato=await UniTask.Delay(500,cancellationToken token).SuppressCancellationThrow();
            
        }
    }

    public bool CanInteractWith()
    {
        var cells = GetCellsInBounds(Data.interactionBounds);
        foreach (var item in cells)
            if (item == PlayerManager.Instance.GetPlayerCell()) return true;

        return false;
    }
    public void Interact()
    {
        MachineUI.Instance.Machine = this;
        MachineUI.Instance.Open(this);
    }
    public RecipeSO Craft(RecipeSO recipe, Bot bot)
    {
        if (!CanCraft(recipe, bot))
            return null;

        return recipe;
    }
    public bool CanCraft(RecipeSO recipe, Bot bot)
    {
        if (bot.Type != BotType.Worker)
            return false;

        foreach (var kvp in recipe.inputs)
        {
            if (!bot.Inventory.Items.ContainsKey(kvp.Key))
                return false;
            if (bot.Inventory.Items[kvp.Key] < kvp.Value)
                return false;
        }

        return true;
    }
}
public enum MachineType
{
    None,
    Advanced_Assembler,
    Alloy_Furnace,
    Assembler,
    Press,
    Smelter,
}
