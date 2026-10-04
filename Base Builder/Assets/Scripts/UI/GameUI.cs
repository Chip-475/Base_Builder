using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public int TotalPowerConsumption;
    public int TotalPowerProduction;

    [Header("Default Screen")]
    [SerializeField] TMP_Text powerCounter;
    [SerializeField] TMP_Text botsCounter;

    [Header("Inventory Menu")]
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] Button openInventoryMenu;
    [SerializeField] Button closeInventoryMenu;

    private void Awake()
    {
        inventoryMenu.SetActive(false);

        openInventoryMenu.onClick.AddListener(() => ToggleStatMenu());
        closeInventoryMenu.onClick.AddListener(() => ToggleStatMenu());
    }
    private void Update()
    {
        powerCounter.text = $"{TotalPowerConsumption} / {TotalPowerProduction}";
        botsCounter.text = $"[insert bot count]";
    }

    void ToggleStatMenu()
    {
        inventoryMenu.SetActive(!inventoryMenu.activeSelf);
    }
}
