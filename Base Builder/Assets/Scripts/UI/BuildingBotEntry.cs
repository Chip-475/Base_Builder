using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBotEntry : MonoBehaviour
{
    public MachineUI UI => MachineUI.Instance;

    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button button;
    
    WorkerBot bot;

    private void Update()
    {
        button.interactable = bot.Occupied == false
            && MachineUI.Instance.Machine != null
            && MachineUI.Instance.Machine.Occupied == false
            && MachineUI.Instance.selectedRecipeEntry != null;
    }
    public void Initialize(WorkerBot bot)
    {
        this.bot = bot;

        icon.sprite = bot.Data.sprite;
        nameText.text = bot.Name;

        button.onClick.AddListener(() => AssignTask());
    }

    void AssignTask()
    {
        bot.CurrentTask = new(bot, UI.Machine, UI.selectedRecipeEntry.recipe, UI.selectedRecipeEntry.amount);
    }
}
