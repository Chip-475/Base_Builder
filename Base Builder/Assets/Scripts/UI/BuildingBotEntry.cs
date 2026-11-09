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
        MachineUI ui = MachineUI.Instance;
        button.interactable = bot != null && !bot.Occupied
            && ui != null && ui.Machine != null && !ui.Machine.Occupied
            && ui.selectedRecipeEntry != null
            && ui.Machine.CanUseWorker(bot)
            && ui.Machine.Data.usableRecipes != null
            && ui.Machine.Data.usableRecipes.Contains(ui.selectedRecipeEntry.recipe);
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
        if (bot == null || UI == null || UI.Machine == null || UI.selectedRecipeEntry == null)
            return;

        RecipeSO recipe = UI.selectedRecipeEntry.recipe;
        if (!UI.Machine.TryAssignWorker(bot, recipe))
            return;

        bot.CurrentTask = new(bot, UI.Machine, recipe, UI.selectedRecipeEntry.Amount);
        UI.BuildBotList();
    }
}
