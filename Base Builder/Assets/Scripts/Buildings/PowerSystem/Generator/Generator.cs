using System;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Building,IInteractable
{
    public new GeneratorData Data => base.Data as GeneratorData;
    public new GeneratorView SceneObj => base.SceneObj as GeneratorView;

    public string id;

    public ResourceSO selected;
    public float Power { get { return selected.fv; } }
    public bool Running = true; // { get; private set; }

    public bool isPowered;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public Dictionary<ResourceSO, float> inventory;
    public Generator(GeneratorData data, GeneratorView sceneObj) : base(data, sceneObj)
    {
        id = Guid.NewGuid().ToString();
        PowerManager.powerGeneratorDB[id] = this;
        PowerManager.instance.RegisterBuilding();

    }
    public override void Destroy()
    {
        PowerManager.powerGeneratorDB.Remove(id);
        PowerManager.instance.UnregisterBuilding();
        BuildingManager.UnregisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj.gameObject);
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
        GeneratorUI.instance.Generator = this;
        GeneratorUI.instance.Open(this);
    }
    public void SwitchState(bool state) //to link to the ui button
    {
        Running = state;
        if (Network != null)
            Network.RefreshPower();
    }
}
