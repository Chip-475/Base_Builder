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
        button.onClick.AddListener(() => MachineUI.Instance.BuildBotList());

        addButton.onClick.AddListener(() => ModifyAmount(1));
        subtractButton.onClick.AddListener(() => ModifyAmount(-1));

        quantityText[outputIcon] = outputIcon.GetComponentInChildren<TMP_Text>();
        foreach (var item in inputIcons)
            quantityText[item] = item.GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (recipe == null || Depot.Inventory == null || addButton == null || subtractButton == null)
            return;

        subtractButton.interactable = Amount > 1;
        int nextAmount = Amount + 1;
        Dictionary<ResourceSO, int> nextInputs = new();
        foreach (var item in recipe.inputs)
            nextInputs[item.Key] = item.Value * nextAmount;

        addButton.interactable = Depot.Inventory.CanRemoveItems(nextInputs);
    }

    public void Build(RecipeSO recipe)
    {
        if (recipe == null)
            throw new Exception("Cannot display a null recipe.");
        if (recipe.inputs == null || recipe.outputs == null ||
            recipe.outputs.Count != 1)
            throw new Exception($"Recipe {recipe.Name} is invalid.");

        var outputs = recipe.outputs.ToArray();
        var inputs = recipe.inputs.ToArray();

        this.recipe = recipe;

        outputIcon.sprite = outputs[0].Key.Sprite;
        quantityText[outputIcon].text = (outputs[0].Value * Amount).ToString();
        for (int i = 0; i < inputIcons.Count; i++)
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
        if (recipe == null || this.Amount + amount <= 0 || Depot.Inventory == null)
            return;

        Dictionary<ResourceSO, int> required = new();
        foreach (var item in recipe.inputs)
            required[item.Key] = item.Value * (this.Amount + amount);

        if (!Depot.Inventory.CanRemoveItems(required))
            return;

        this.Amount += amount;
        Build(recipe);
    }
}
