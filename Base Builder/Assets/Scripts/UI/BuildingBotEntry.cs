using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBotEntry : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button button;
    
    Bot bot;

    private void Update()
    {
        button.interactable = bot.Occupied == false && MachineUI.Instance.Machine != null && MachineUI.Instance.Machine.Occupied == false && MachineUI.Instance.selectedRecipeEntry != null;
    }
    public void Initialize(Bot bot)
    {
        this.bot = bot;

        icon.sprite = bot.BotData.sprite;
        nameText.text = bot.Name;

        button.onClick.AddListener(() => BotManager.ProcessRequest(new(bot, MachineUI.Instance.Machine, MachineUI.Instance.selectedRecipeEntry.recipe)));
    }
}
