using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Button))]
public class RecipeEntry : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Button button;
    [SerializeField] Button addButton;
    [SerializeField] Button subtractButton;
    [SerializeField] Image craftedItem;
    [SerializeField] List<Image> requestedItems;

    [Header("Runtime Data")]
    public RecipeSO recipe;
    public int amount;

    readonly Dictionary<Image, TMP_Text> quantityText = new();

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => MachineUI.Instance.selectedRecipeEntry = this);
        button.onClick.AddListener(() => MachineUI.Instance.BuildInfoPanel(this));

        addButton.onClick.AddListener(() => ModifyAmount(1));
        subtractButton.onClick.AddListener(() => ModifyAmount(-1));

        foreach (var item in requestedItems)
            quantityText[item] = item.GetComponentInChildren<TMP_Text>();
    }
    private void Update()
    {
        addButton.interactable = true;
        subtractButton.interactable = true;

        foreach (var item in recipe.inputs)
            if (!Depot.Inventory.CanRemoveItem(item.Key, item.Value * amount))
                addButton.interactable = false;

        foreach (var item in recipe.outputs)
            if (!Depot.Inventory.CanAddItem(item.Key, item.Value * amount))
                addButton.interactable = false;
    }

    public void Build(RecipeSO recipe)
    {
        if (recipe.inputs.Count > 3 || recipe.outputs.Count > 1)
            throw new Exception($"Recipe {recipe.Name} is invalid.");

        this.recipe = recipe;
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            Image currentEntry = requestedItems[i];
            var Keys = recipe.inputs.Keys.ToArray();
            var Values = recipe.outputs.Keys.ToArray();

            currentEntry.sprite = Keys[i].Sprite;
            quantityText[currentEntry].text = recipe.inputs[Keys[i]].ToString();
        }
    }

    void ModifyAmount(int amount)
    {
        if (this.amount + amount <= 0)
            return;

        foreach (var item in recipe.inputs)
            if (!Depot.Inventory.CanRemoveItem(item.Key, item.Value * (this.amount + amount)))
                return;
        foreach (var item in recipe.outputs)
            if (!Depot.Inventory.CanAddItem(item.Key, item.Value * (this.amount + amount)))
                return;

        this.amount += amount;
    }
}