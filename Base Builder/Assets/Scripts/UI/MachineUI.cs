using UnityEngine;
using UnityEngine.UI;

public class MachineUI : MonoBehaviour
{
    public static MachineUI Instance { get; private set; }

    Machine currentSelected;
    public Machine Selected
    {
        get { return currentSelected; }
        set { currentSelected = value; Build(value); }
    }
    public bool IsActive => panel.activeSelf;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] RecipeEntry recipeEntryPrefab;

    [Header("Right side")]
    [SerializeField] GameObject recipeListContent;

    [Header("Upper Left Side")]
    [SerializeField] Button backButton;
    [SerializeField] Image recipeIcon;

    [Header("Lower Left Side")]
    [SerializeField] Image craftedItemIcon;

    private void Awake()
    {
        Instance = this;

        backButton.onClick.AddListener(() => panel.SetActive(false));
    }

    public void Build(Machine machine)
    {
        Wipe();

        // Left Side
        foreach (var recipe in machine.Data.usableRecipes)
        {
            RecipeEntry entry = Instantiate(recipeEntryPrefab, recipeListContent.transform);
            entry.Build(recipe);
        }

        // Upper Right Side
    }

    void Wipe()
    {
        // Left Side
        foreach (Transform child in recipeListContent.transform)
            Destroy(child.gameObject);
    }
}