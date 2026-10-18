using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GeneratorUI : MonoBehaviour
{
    public static GeneratorUI Instance { get; private set; }

    public bool IsActive => panel.activeSelf;
    Generator generator;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button backButton;
    [SerializeField] FuelEntry fuelEntryPrefab;
    [SerializeField] BuildingBotEntry botEntryPrefab;

    [Header("Recipe List")]
    [SerializeField] GameObject fuelListContent;

    [Header("Bot List")]
    [SerializeField] GameObject botListContent;

    [Header("Info Panel")]
    [SerializeField] Image selectedFuelIcon;
    [SerializeField] TMP_Text selectedFuelName;
    [SerializeField] TMP_Text selectedFuelAmount;

    [Header("Runtime")]
    public Generator Generator { get => generator; set => generator = value; }
    public FuelEntry selectedFuelEntry;

    private void Awake()
    {
        Instance = this;

        backButton.onClick.AddListener(() => panel.SetActive(false));
        backButton.onClick.AddListener(() => selectedFuelEntry = null);
    }

    public void Open(Generator generator)
    {
        panel.SetActive(true);
        BuildFuelList(generator);
        BuildBotList();
    }

    public void BuildFuelList(Generator generator)
    {
        WipeRecipeList();

        foreach (var fuel in generator.Data.allowedFuels)
        {
            FuelEntry entry = Instantiate(fuelEntryPrefab, fuelListContent.transform);
            entry.Build(fuel);
        }
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
    public void BuildInfoPanel(FuelEntry fuelEntry)
    {
        selectedFuelIcon.sprite = fuelEntry.recipe.Sprite;
        selectedFuelName.text = fuelEntry.recipe.Name;
        selectedFuelAmount.text = fuelEntry.recipe.Desc;
    }
    
    void WipeRecipeList()
    {
        foreach (Transform child in fuelListContent.transform)
            Destroy(child.gameObject);
    }
    void WipeBotList()
    {
        foreach (Transform child in botListContent.transform)
            Destroy(child.gameObject);
    }
}
