using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MineralNode : Building, IInteractable, IWorkplace
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public new MineralNodeView SceneObj => base.SceneObj as MineralNodeView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public MineralNode(MineralNodeData data, MineralNodeView sceneObj) : base(data, sceneObj)
    {
        
    }
    public override void Destroy()
    {
        BuildingManager.RegisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj);
    }

    // IInteractable
    public void Interact()
    {
        if (!CanInteractWith())
            return;

        // open UI
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
        KeyValuePair<ResourceSO, int> output = new(Data.resource, Data.baseAmountGiven * (int)Data.purity);
        request.bot.Inventory.AddItem(output.Key, output.Value);
    }
    public bool CanWork(BotManager.WorkRequest request)
    {
        if (request.bot.Type != BotType.Miner)
            return false;

        return true;
    }
    public bool HasRequiredItems(BotManager.WorkRequest request)
    {
        KeyValuePair<ResourceSO, int> output = new(Data.resource, Data.baseAmountGiven * (int)Data.purity);

        if (request.bot.MiningLevel < output.Key.toughness)
            return false;
        if (!request.bot.Inventory.CanAddItem(output.Key, output.Value))
            return false;

        return true;
    }
}
