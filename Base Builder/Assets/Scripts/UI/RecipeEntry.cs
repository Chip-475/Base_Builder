using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RecipeEntry : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Button button;
    [SerializeField] Button addButton;
    [SerializeField] Button subtractButton;
    [Space]
    [SerializeField] Image outputIcon;
    [SerializeField] List<Image> inputsIcons;

    [Header("Runtime Data")]
    public RecipeSO recipe;
    public int Amount { get; set; } = 1;

    readonly Dictionary<Image, TMP_Text> quantityText = new();

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => MachineUI.Instance.selectedRecipeEntry = this);
        button.onClick.AddListener(() => MachineUI.Instance.BuildInfoPanel(this));

        addButton.onClick.AddListener(() => ModifyAmount(1));
        subtractButton.onClick.AddListener(() => ModifyAmount(-1));

        quantityText[outputIcon] = outputIcon.GetComponentInChildren<TMP_Text>();
        foreach (var item in inputsIcons)
            quantityText[item] = item.GetComponentInChildren<TMP_Text>();
    }
    private void Update()
    {
        addButton.interactable = true;
        subtractButton.interactable = true;

        foreach (var item in recipe.inputs)
            if (!Depot.Inventory.CanRemoveItem(item.Key, item.Value * Amount))
                addButton.interactable = false;

        foreach (var item in recipe.outputs)
            if (!Depot.Inventory.CanAddItem(item.Key, item.Value * Amount))
                addButton.interactable = false;
    }

    public void Build(RecipeSO recipe)
    {
        if (recipe.inputs.Count > 3 || recipe.outputs.Count > 1)
            throw new Exception($"Recipe {recipe.Name} is invalid.");

        var outputs = recipe.outputs.ToArray();
        var inputs = recipe.inputs.ToArray();

        this.recipe = recipe;
        outputIcon.sprite = outputs[0].Key.Sprite;
        quantityText[outputIcon].text = (outputs[0].Value * Amount).ToString();
        for (int i = 0; i < 3; i++)
        {
            if (i >= recipe.inputs.Count)
            {
                inputsIcons[i].gameObject.SetActive(false);
                continue;
            }

            inputsIcons[i].gameObject.SetActive(true);
            inputsIcons[i].sprite = inputs[i].Key.Sprite;
            quantityText[inputsIcons[i]].text = (inputs[i].Value * Amount).ToString();
        }
    }

    void ModifyAmount(int amount)
    {
        if (this.Amount + amount <= 0)
            return;

        foreach (var item in recipe.inputs)
            if (!Depot.Inventory.CanRemoveItem(item.Key, item.Value * (this.Amount + amount)))
                return;

        foreach (var item in recipe.outputs)
            if (!Depot.Inventory.CanAddItem(item.Key, item.Value * (this.Amount + amount)))
                return;

        this.Amount += amount;
        Build(recipe);
    }
}