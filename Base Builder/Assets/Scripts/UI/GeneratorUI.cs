using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUI : MonoBehaviour
{
    public static GeneratorUI Instance { get; private set; }

    public bool IsActive => panel.activeSelf;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button backButton;
    [SerializeField] FuelEntry fuelEntryPrefab;
    [SerializeField] BuildingBotEntry botEntryPrefab;

    [Header("Fuel List")]
    [SerializeField] GameObject fuelListContent;

    [Header("Bot List")]
    [SerializeField] GameObject botListContent;

    [Header("Info Panel")]
    [SerializeField] TMP_Text status;
    [SerializeField] Button switchButton;
    [Space]
    [SerializeField] Image selectedFuelIcon;
    [SerializeField] TMP_Text selectedFuelName;
    [SerializeField] TMP_Text selectedFuelAmount;
    [Space]
    [SerializeField] TMP_Text genProduction;
    [SerializeField] TMP_Text genConsuption;
    [Space]
    [SerializeField] TMP_Text netProduction;
    [SerializeField] TMP_Text netConsuption;

    [Header("Runtime")]
    public Generator Generator { get; set ; }
    public FuelEntry selectedEntry;

    private void Awake()
    {
        Instance = this;

        backButton.onClick.AddListener(() => panel.SetActive(false));
        backButton.onClick.AddListener(() => selectedEntry = null);
        switchButton.onClick.AddListener(() => Generator.SwitchState(!Generator.Running));
    }
    private void Update()
    {
        if (!IsActive)
            return;

        status.text = "Status: \n" + (Generator.Running ? "On" : "Off");
        netConsuption.text = Generator.Network != null ? "Network \n Consuption \n " + Generator.Network.Consumption.ToString() : "Connect to a Power Pole\n to create a network";
        netProduction.text = Generator.Network != null ? "Network \n Production \n " + Generator.Network.Generation.ToString()  : "Connect to a Power Pole\n to create a network";
        genConsuption.text = "placeholder";
        genProduction.text = "placeholder";
    }

    public void Open(Generator generator)
    {
        Generator = generator;
        panel.SetActive(true);
        BuildFuelList(generator);
        BuildBotList();
    }
    
    public void BuildInfoPanel(FuelEntry fuelEntry)
    {
        selectedFuelIcon.sprite = fuelEntry.fuel.Sprite;
        if(Generator.fuelInUse!=null)selectedFuelName.text = Generator.fuelInUse.Name;
        selectedFuelAmount.text = Generator.Quantity.ToString();
    }

    public void BuildFuelList(Generator generator)
    {
        WipeFuelList();

        foreach (var fuel in generator.Data.allowedFuels)
        {
            FuelEntry entry = Instantiate(fuelEntryPrefab, fuelListContent.transform);
            entry.Build(fuel);
        }
    }
    public void BuildBotList()
    {
        WipeBotList();

        foreach (WorkerBot bot in BotManager.Bots.OfType<WorkerBot>())
        {
            BuildingBotEntry go = Instantiate(botEntryPrefab, botListContent.transform);
            go.Initialize(bot);
        }
    }

    void WipeFuelList()
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
