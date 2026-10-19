using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Building, IInteractable, IWorkplace
{
    public new GeneratorData Data => base.Data as GeneratorData;
    public new GeneratorView SceneObj => base.SceneObj as GeneratorView;

    [Header("Runtime")]
    public string id;
    [Space]
    public ResourceSO fuelInUse;
    public Dictionary<ResourceSO, int> Inventory { get; private set; } = new();
    public int Quantity { get { return Inventory[fuelInUse]; } private set { } }
    public float Power { get { return fuelInUse.fuelValue; } }
    public bool Running { get; private set; }
    Coroutine consumer;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public Generator(GeneratorData data, GeneratorView sceneObj) : base(data, sceneObj)
    {
        id = Guid.NewGuid().ToString();
        PowerManager.powerGeneratorDB[id] = this;
        PowerManager.instance.RegisterBuilding();
        InteractionManager.Instance.interactables.Add(this);
    }
    public override void Destroy()
    {
        PowerManager.powerGeneratorDB.Remove(id);
        PowerManager.instance.UnregisterBuilding();
        BuildingManager.UnregisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public void StartGenerator()
    {
        consumer = SceneObj.StartCoroutine(ConsumeFuel(fuelInUse));
    }
    public void StopGenerator()
    {
        SceneObj.StopCoroutine(consumer);
        Running = false;
    }
    public void SwitchState(bool state) //to link to the ui button
    {
        Running = state;
        if (Network != null)
            Network.RefreshPower();
    }
    IEnumerator ConsumeFuel(ResourceSO fuel)
    {
        if (!Inventory.TryGetValue(fuel, out _))
            StopGenerator();

        Inventory[fuel] -= 1;
        yield return new WaitForSeconds(fuel.timeToConsume);
        consumer = SceneObj.StartCoroutine(ConsumeFuel(fuel));
    }

    // IInteractable
    public void Interact()
    {
        if (!CanInteractWith())
            return;

        GeneratorUI.Instance.Generator = this;
        GeneratorUI.Instance.Open(this);
    }
    public bool CanInteractWith()
    {
        var cells = GetCellsInBounds(Data.interactionBounds);
        foreach (var item in cells)
            if (item == PlayerManager.Instance.GetPlayerCell()) return true;

        return false;
    }

    // IWorkplace
    /// <summary> MUST have only 1 input and 0 outputs. </summary>
    public RecipeSO Work(RecipeSO recipe, Bot bot)
    {
        if (!CanWork(recipe, bot))
            return null;

        return recipe;
    }
    /// <summary> MUST have only 1 input and 0 outputs. </summary>
    public bool CanWork(RecipeSO recipe, Bot bot)
    {
        if (bot.Type == BotType.Carrier)
            return false;

        return false;
    }
    /// <summary> MUST have only 1 input and 0 outputs. </summary>
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
