using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineUI : MonoBehaviour
{
    public static MachineUI Instance { get; private set; }

    public bool IsActive => panel.activeSelf;
    Machine machine;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button backButton;
    [SerializeField] RecipeEntry recipeEntryPrefab;
    [SerializeField] BuildingBotEntry botEntryPrefab;

    [Header("Recipe List")]
    [SerializeField] GameObject recipeListContent;

    [Header("Info Panel")]
    [SerializeField] Image selectedRecipeIcon;
    [SerializeField] TMP_Text selectedRecipeName;
    [SerializeField] TMP_Text selectedRecipeDesc;

    [Header("Bot List")]
    [SerializeField] GameObject botListContent;

    [Header("Runtime")]
    public Machine Machine { get => machine; set => machine = value; }
    public RecipeEntry selectedRecipeEntry;

    private void Awake()
    {
        Instance = this;

        if (backButton != null)
        {
            backButton.onClick.AddListener(() => panel.SetActive(false));
            backButton.onClick.AddListener(() =>
            {
                selectedRecipeEntry = null;
                BuildBotList();
            });
        }
    }

    public void Open(Machine machine)
    {
        if (machine == null || machine.Data == null)
            return;

        Machine = machine;
        selectedRecipeEntry = null;
        panel.SetActive(true);
        selectedRecipeIcon.sprite = null;
        selectedRecipeName.text = string.Empty;
        selectedRecipeDesc.text = string.Empty;
        BuildRecipeList(machine);
        BuildBotList();
    }
    public void BuildRecipeList(Machine machine)
    {
        WipeRecipeList();

        if (machine == null || machine.Data == null || machine.Data.usableRecipes == null)
            return;

        foreach (var recipe in machine.Data.usableRecipes)
        {
            if (recipe == null ||
                (!recipe.unlockedByDefault && !RecipeManager.UnlockedRecipes.Contains(recipe.ID)))
                continue;

            RecipeEntry entry = Instantiate(recipeEntryPrefab, recipeListContent.transform);
            entry.Build(recipe);
        }
    }
    public void BuildInfoPanel(RecipeEntry recipeEntry)
    {
        if (recipeEntry == null || recipeEntry.recipe == null)
            return;

        selectedRecipeIcon.sprite = recipeEntry.recipe.Sprite;
        selectedRecipeName.text = recipeEntry.recipe.Name;

        StringBuilder details = new();
        if (!string.IsNullOrWhiteSpace(recipeEntry.recipe.Desc))
            details.AppendLine(recipeEntry.recipe.Desc.Trim());

        details.AppendLine("Inputs:");
        foreach (var item in recipeEntry.recipe.inputs)
            details.AppendLine($"{item.Value} × {item.Key.Name}");

        details.AppendLine("Output:");
        foreach (var item in recipeEntry.recipe.outputs)
            details.AppendLine($"{item.Value} × {item.Key.Name}");

        details.Append($"Time: {recipeEntry.recipe.completionTime:0.#} s  •  Energy: {recipeEntry.recipe.energyCost}");
        selectedRecipeDesc.text = details.ToString();
    }
    public void BuildBotList()
    {
        WipeBotList();

        if (machine == null || machine.Data == null)
            return;

        List<WorkerBot> needed = BotManager.Bots.OfType<WorkerBot>()
            .Where(bot => bot != null && machine.CanUseWorker(bot))
            .ToList();
        needed.Sort((a, b) => a.Occupied.CompareTo(b.Occupied));
        foreach (WorkerBot bot in needed)
        {
            BuildingBotEntry go = Instantiate(botEntryPrefab, botListContent.transform);
            go.Initialize(bot);
        }
    }

    void WipeRecipeList()
    {
        foreach (Transform child in recipeListContent.transform)
            Destroy(child.gameObject);
    }
    void WipeBotList()
    {
        foreach(Transform child in botListContent.transform)
            Destroy(child.gameObject);
    }
}
