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
    [SerializeField] List<Image> inputIcons;

    [Header("Runtime Data")]
    public RecipeSO recipe;
    public int Amount { get; set; } = 1;

    readonly Dictionary<Image, TMP_Text> quantityText = new();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() => MachineUI.Instance.selectedRecipeEntry = this);
        button.onClick.AddListener(() => MachineUI.Instance.BuildInfoPanel(this));

        addButton.onClick.AddListener(() => ModifyAmount(1));
        subtractButton.onClick.AddListener(() => ModifyAmount(-1));

        quantityText[outputIcon] = outputIcon.GetComponentInChildren<TMP_Text>();
        foreach (var item in inputIcons)
            quantityText[item] = item.GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
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

        quantityText[outputIcon].text = (outputs[0].Value * Amount).ToString();
        for (int i = 0; i < 3; i++)
        {
            if (i >= recipe.inputs.Count)
            {
                inputIcons[i].gameObject.SetActive(false);
                continue;
            }

            inputIcons[i].gameObject.SetActive(true);
            inputIcons[i].sprite = inputs[i].Key.Sprite;
            quantityText[inputIcons[i]].text = (inputs[i].Value * Amount).ToString();
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