using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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

        backButton.onClick.AddListener(() => panel.SetActive(false));
        backButton.onClick.AddListener(() => selectedRecipeEntry = null);
    }

    public void Open(Machine machine)
    {
        if(machine==null)
        {
            Debug.Log("machine è null");
            return;
        }
        if(machine.Data==null)
        {
            Debug.Log("machine data è null");
            return;
        }
        Open(machine.Data.usableRecipes);
        panel.SetActive(true);
        BuildRecipeList(machine.Data.usableRecipes);
        BuildBotList();
    }

    public void Open(List<RecipeSO> ricette)
    {
        if(ricette==null)
        {
            Debug.Log("lista null");
            return;
        }
        panel.SetActive(true);
        BuildRecipeList(ricette);
    }

    public void BuildRecipeList(List<RecipeSO>ricette)
    {
        WipeRecipeList();

        /*
        foreach(var recipe in machine.Data.usableRecipes)
=======

        if (machine == null)
>>>>>>> giulio'sWork
        {
            Debug.LogError("Cannot build the machine recipe list because the machine is null.", this);
            return;
        }

        if (machine.Data == null)
        {
            Debug.LogError("Cannot build the machine recipe list because the machine has no MachineData.", this);
            return;
        }

        if (machine.Data.usableRecipes == null)
        {
            Debug.LogError($"Machine data '{machine.Data.name}' has no usable recipe list.", machine.Data);
            return;
        }

        foreach (var recipe in machine.Data.usableRecipes)
        {
            if (recipe == null)
                continue;

            RecipeEntry entry = Instantiate(recipeEntryPrefab, recipeListContent.transform);
            entry.Build(recipe);
        }
        */
        foreach(RecipeSO recipe in ricette)
        {
            if(recipe==null) continue;
            RecipeEntry entry=Instantiate(recipeEntryPrefab,recipeListContent.transform);
            entry.Build(recipe);
        }
    }
    public void BuildInfoPanel(RecipeEntry recipeEntry)
    {
        selectedRecipeIcon.sprite = recipeEntry.recipe.Sprite;
        selectedRecipeName.text = recipeEntry.recipe.Name;
        selectedRecipeDesc.text = recipeEntry.recipe.Desc;
    }
    public void BuildBotList()
    {
        WipeBotList();

        foreach (Bot bot in BotManager.Bots)
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
