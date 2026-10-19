using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Machine : Building, IInteractable, IWorkplace
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public bool Powered { get; private set; }
    public float RequestedPower { get; private set; }
    public MachineType Type { get; protected set; } = MachineType.None;

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

    // IInteractable
    public void Interact()
    {
        if (!CanInteractWith())
            return;

        MachineUI.Instance.Machine = this;
        MachineUI.Instance.Open(this);
    }
    public bool CanInteractWith()
    {
        var cells = GetCellsInBounds(Data.interactionBounds);
        foreach (var item in cells)
            if (item == PlayerManager.Instance.GetPlayerCell()) return true;

        return false;
    }
    
    // IWorkplace
    public RecipeSO Work(RecipeSO recipe, Bot bot)
    {
        if (!CanWork(recipe, bot))
            return null;
        if (!HasRequiredItems(recipe, bot))
            return null;

        return recipe;
    }
    public bool CanWork(RecipeSO recipe, Bot bot)
    {
        if (bot.Type != BotType.Worker)
            return false;

        return true;
    }
    public bool HasRequiredItems(RecipeSO recipe, Bot bot)
    {
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
