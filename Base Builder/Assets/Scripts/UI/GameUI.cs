using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

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
        foreach (var bot in BotManager.Bots)
        {
            Debug.Log(bot.Occupied);
        }

        powerCounter.text = $"{PowerManager.Consuption} / {PowerManager.Production}";
        botsCounter.text = $"{BotManager.EmployedBots} / {BotManager.BotCount}";
    }

    void ToggleStatMenu()
    {
        inventoryMenu.SetActive(!inventoryMenu.activeSelf);
    }
}
