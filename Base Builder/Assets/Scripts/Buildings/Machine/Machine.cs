using Cysharp.Threading.Tasks;
using System;
using System.Collections;
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
        var cells = GetCellsInBounds(InteractionBounds);
        foreach (var item in cells)
            if (item == PlayerManager.Instance.GetPlayerCell()) return true;

        return false;
    }

    // IWorkplace
    public IEnumerator Work(BotManager.WorkRequest request)
    {
        if (!CanWork(request))
            yield break;

        yield return new WaitForSeconds(request.recipe.completionTime);
        foreach (var kvp in request.recipe.outputs)
            request.bot.Inventory.AddItem(kvp.Key, kvp.Value);
    }
    public bool CanWork(BotManager.WorkRequest request)
    {
        if (request.bot.Type != BotType.Worker)
            return false;

        return true;
    }
    public bool HasRequiredItems(BotManager.WorkRequest request)
    {
        foreach (var kvp in request.recipe.inputs)
        {
            if (!request.bot.Inventory.Items.ContainsKey(kvp.Key))
                return false;
            if (request.bot.Inventory.Items[kvp.Key] < kvp.Value)
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
