using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GeneratorUI : MonoBehaviour
{
    public static GeneratorUI instance { get; private set; }

    public bool IsActive => panel.activeSelf;
    Generator generator;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button backButton;
    [SerializeField] FuelEntry recipeEntryPrefab;
    [SerializeField] BuildingBotEntry botEntryPrefab;

    [Header("Recipe List")]
    [SerializeField] GameObject recipeListContent;

    [Header("Info Panel")]
    [SerializeField] Image selectedFuelIcon;
    [SerializeField] TMP_Text selectedFuelName;
    [SerializeField] TMP_Text selectedFuelAmount;

    [Header("Bot List")]
    [SerializeField] GameObject botListContent;

    [Header("Runtime")]
    public Generator Generator { get => generator; set => generator = value; }
    public FuelEntry selectedFuelEntry;

    private void Awake()
    {
        instance = this;

        backButton.onClick.AddListener(() => panel.SetActive(false));
        backButton.onClick.AddListener(() => selectedFuelEntry = null);
    }

    public void Open(Generator gen)
    {
        panel.SetActive(true);
        BuildRecipeList(gen);
        BuildBotList();
    }
    public void BuildRecipeList(Generator gen)
    {
        WipeRecipeList();

        if (gen == null)
        {
            Debug.LogError("Cannot build the machine recipe list because the machine is null.", this);
            return;
        }

        if (gen.Data == null)
        {
            Debug.LogError("Cannot build the machine recipe list because the machine has no MachineData.", this);
            return;
        }

        if (gen.Data.allowedFuels == null)
        {
            //Debug.LogError($"Machine data '{machine.Data.name}' has no usable recipe list.", machine.Data);
            return;
        }

        foreach (var fuel in gen.Data.allowedFuels)
        {
            if (fuel == null)
                continue;

            FuelEntry entry = Instantiate(recipeEntryPrefab, recipeListContent.transform);
            entry.Build(fuel);
        }
    }
    public void BuildInfoPanel(FuelEntry fuelEntry)
    {
        selectedFuelIcon.sprite = fuelEntry.recipe.Sprite;
        selectedFuelName.text = fuelEntry.recipe.Name;
        selectedFuelAmount.text = fuelEntry.recipe.Desc;
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
        foreach (Transform child in botListContent.transform)
            Destroy(child.gameObject);
    }
}
