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

        backButton.onClick.AddListener(() => panel.SetActive(false));
        backButton.onClick.AddListener(() => selectedRecipeEntry = null);
    }

    public void Open(Machine machine)
    {
        panel.SetActive(true);
        BuildRecipeList(machine);
        BuildBotList();
    }
    public void BuildRecipeList(Machine machine)
    {
        WipeRecipeList();

        if (machine == null)
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
