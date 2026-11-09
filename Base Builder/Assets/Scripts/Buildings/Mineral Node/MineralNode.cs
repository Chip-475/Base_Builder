using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MineralNode : Building, IInteractable
{
    public new MineralNodeData Data => base.Data as MineralNodeData;
    public new MineralNodeView SceneObj => base.SceneObj as MineralNodeView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);
    bool disposed;

    public MineralNode(MineralNodeData data, MineralNodeView sceneObj) : base(data, sceneObj)
    {
        
    }
    public override void Destroy()
    {
        Dispose();
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        FreeUpCells(GetCellsInBounds(GetBounds()));
        BuildingManager.UnregisterBuilding(this);
        if (PowerManager.instance != null)
            PowerManager.instance.UnregisterBuilding();
    }

    // IWorkplace
    public override Dictionary<ResourceSO, int> GetWorkOutputs()
    {
        return new()
        {
            [Data.resource] = Data.baseAmountGiven * (int)Data.purity
        };
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
}
